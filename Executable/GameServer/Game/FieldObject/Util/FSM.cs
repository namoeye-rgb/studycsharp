using System;
using System.Collections.Generic;

namespace GameServer
{
    /// <summary> Action Finite State Machine State의 key 정의 </summary>
    [Flags]
    public enum ACTIONFSMSTATE
    {
        /// <summary> 오브젝트가 생성된 상태 </summary>
        BORN,
        /// <summary> 대기 </summary>
        IDLE,
        /// <summary> 공격 </summary>
        ATTACK,
        /// <summary> 피격 </summary>
        HIT,
        /// <summary> 달리기 </summary>
        RUN,
        /// <summary> 점프 </summary>
        JUMP,
        /// <summary> 죽음 </summary>
        DEAD,
        /// <summary> 넉백 </summary>
        KNOCKBACK,
        /// <summary> 넉다운 </summary>
        KNOCKDOWN,
        NONE,
    }

    public enum JumpState
    {
        Start,
        Fall,
        End,
    }

    public enum AttackState
    {
        Start,
        Fall,
        End,
    }

    /// <summary>
    /// Dictionary에서 Enum을 key로 사용하였을 경우 boxing이 일어나기 때문에
    /// boxing 회피용 EqualityComparer를 새로 선언해서 사용해야함.
    /// http://stackoverflow.com/questions/26280788/dictionary-enum-key-performance
    /// </summary>
    public struct ACTIONFSMSTATEComparer : IEqualityComparer<ACTIONFSMSTATE>
    {
        public bool Equals(ACTIONFSMSTATE x, ACTIONFSMSTATE y)
        {
            return x == y;
        }

        public int GetHashCode(ACTIONFSMSTATE obj)
        {
            return (int)obj;
        }
    }

    /// <summary> 상태머신의 상태표에 따라서 전이 가능 여부를 확인 </summary>
    public static class ActionFSMRule
    {
        /// <summary> 전이 규칙에 사용할 Key들 </summary>
        public static readonly ACTIONFSMSTATE[] TransitionKeys = new ACTIONFSMSTATE[] {
            // 우선 순위 순서로 정렬
            ACTIONFSMSTATE.BORN,
            ACTIONFSMSTATE.DEAD,
            ACTIONFSMSTATE.KNOCKDOWN,
            ACTIONFSMSTATE.KNOCKBACK,
            ACTIONFSMSTATE.ATTACK,
            ACTIONFSMSTATE.JUMP,
            ACTIONFSMSTATE.RUN,
            ACTIONFSMSTATE.IDLE,
        };

        /// <summary> key상태에서 전이 가능한 상태를 가지고 있음 </summary>
        private static readonly Dictionary<ACTIONFSMSTATE, HashSet<ACTIONFSMSTATE>> m_transitionRules = new Dictionary<ACTIONFSMSTATE, HashSet<ACTIONFSMSTATE>>((int)ACTIONFSMSTATE.NONE) {
            { ACTIONFSMSTATE.BORN,
                new HashSet<ACTIONFSMSTATE> { ACTIONFSMSTATE.DEAD, ACTIONFSMSTATE.IDLE }
            },
            { ACTIONFSMSTATE.DEAD,
                new HashSet<ACTIONFSMSTATE> { ACTIONFSMSTATE.IDLE }
            },
            { ACTIONFSMSTATE.KNOCKDOWN,
                new HashSet<ACTIONFSMSTATE> { ACTIONFSMSTATE.DEAD, ACTIONFSMSTATE.IDLE }
            },
            { ACTIONFSMSTATE.KNOCKBACK,
                new HashSet<ACTIONFSMSTATE> { ACTIONFSMSTATE.DEAD }
            },
            { ACTIONFSMSTATE.ATTACK,
                new HashSet<ACTIONFSMSTATE> { ACTIONFSMSTATE.DEAD, ACTIONFSMSTATE.IDLE }
            },
            { ACTIONFSMSTATE.RUN,
                new HashSet<ACTIONFSMSTATE> { ACTIONFSMSTATE.DEAD, ACTIONFSMSTATE.KNOCKDOWN, ACTIONFSMSTATE.KNOCKBACK, ACTIONFSMSTATE.ATTACK, ACTIONFSMSTATE.JUMP, ACTIONFSMSTATE.IDLE }
            },
            { ACTIONFSMSTATE.IDLE,
                new HashSet<ACTIONFSMSTATE> { ACTIONFSMSTATE.DEAD, ACTIONFSMSTATE.KNOCKDOWN, ACTIONFSMSTATE.KNOCKBACK, ACTIONFSMSTATE.ATTACK, ACTIONFSMSTATE.JUMP, ACTIONFSMSTATE.RUN }
            },
        };
        public static bool IsChangeAbleToState(ACTIONFSMSTATE currentState, ACTIONFSMSTATE changeState)
        {
            var result = m_transitionRules.TryGetValue(currentState, out var state);
            if (result) {
                result = state.Contains(changeState);
            }
            return result;
        }

        public static ACTIONFSMSTATE RequestStateTransition(ACTIONFSMSTATE currentState, Dictionary<ACTIONFSMSTATE, bool> requestTransition)
        {
            var requestNextStates = ACTIONFSMSTATE.NONE;
            var result = m_transitionRules.TryGetValue(currentState, out var current);
            if (!result) {
                return requestNextStates;
            }

            foreach (var state in requestTransition) {
                // 요청 상태 확인
                if (!state.Value || !current.Contains(state.Key)) {
                    continue;
                }

                requestNextStates = state.Key;
                break;
            }

            return requestNextStates;
        }
    }

    public class StateMachine<TObject, TStateKey>
    {
        public delegate void StateChangeHandler(object sender, StateMachineEventArgs e);
        public delegate void StateStartedHandler(object sender, StateMachineEventArgs e);

        public struct StateMachineEventArgs
        {

            public TStateKey Prev;
            public TStateKey Next;

            public override string ToString()
            {
                return $"{Prev} -> {Next}";
            }
        }

        // state base class
        public class StateBase
        {
            public TStateKey Key { get; set; }
            public double ElapsedTime { get; private set; }
            public StateMachine<TObject, TStateKey> StateMachine { get; set; }

            public void Start(TStateKey prevState)
            {
                ElapsedTime = 0.0f;
                OnStart(StateMachine.Owner, prevState);
            }

            public void Update(double deltaTime)
            {
                OnUpdate(StateMachine.Owner, deltaTime);
                ElapsedTime += deltaTime;
            }

            public void End(TStateKey nextState)
            {
                OnEnd(StateMachine.Owner, nextState);
            }

            public virtual void ResetElapsedTime()
            {
                ElapsedTime = 0.0f;
            }

            protected virtual void OnStart(TObject owner, TStateKey prevState) { }
            protected virtual void OnUpdate(TObject owner, double deltaTime) { }
            protected virtual void OnEnd(TObject owner, TStateKey nextState) { }
        }

        // event
        public event StateChangeHandler OnStateChangeEvent;
        public event StateStartedHandler OnStateStartedEvent;

        // property
        public TObject Owner { get; private set; }
        public TStateKey StateKey {
            get {
                if (m_current != null) {
                    return m_current.Key;
                }

                return default(TStateKey);
            }
        }
        public double StateElapsedTime {
            get {
                if (m_current != null) {
                    return m_current.ElapsedTime;
                }

                return 0.0;
            }
        }
        // variable
        private StateBase m_current;
        private readonly Dictionary<TStateKey, StateBase> m_states;
        private readonly IEqualityComparer<TStateKey> m_comparer;

        // functions
        public StateMachine(TObject owner)
        {
            Owner = owner;
            m_states = new Dictionary<TStateKey, StateBase>();
        }

        public StateMachine(TObject owner, IEqualityComparer<TStateKey> keyComparer)
        {
            Owner = owner;
            m_states = new Dictionary<TStateKey, StateBase>(keyComparer);
            m_comparer = keyComparer;
        }

        public StateMachine(TObject owner, int capacity, IEqualityComparer<TStateKey> keyComparer)
        {
            Owner = owner;
            m_states = new Dictionary<TStateKey, StateBase>(capacity, keyComparer);
            m_comparer = keyComparer;
        }


        public void AddState(TStateKey stateKey, StateBase state)
        {
            if (m_states.ContainsKey(stateKey)) {
                m_states.Remove(stateKey);
            }
            state.StateMachine = this;
            state.Key = stateKey;
            m_states.Add(stateKey, state);
        }

        public void Update(double elapsedTime)
        {
            m_current?.Update(elapsedTime);
        }

        public bool ChangeState(TStateKey newState)
        {
            var result = false;
            result = m_states.TryGetValue(newState, out StateBase nextState);
            if (result) {
                var oldState = default(TStateKey);
                oldState = m_current.Key;

                var args = new StateMachineEventArgs {
                    Prev = oldState,
                    Next = newState
                };

                OnStateChangeEvent?.Invoke(this, args);
                m_current.End(newState);

                m_current = nextState;
                m_current.Start(oldState);

                OnStateStartedEvent?.Invoke(this, args);
            }
            return result;
        }

        public void ResetElapsed()
        {
            m_current?.ResetElapsedTime();
        }
    }
}
