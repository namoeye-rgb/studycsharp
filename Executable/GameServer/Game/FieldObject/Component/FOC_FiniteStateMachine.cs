using System.Collections.Generic;

namespace GameServer
{
    using FieldObjectActionFSM = StateMachine<FieldObject, ACTIONFSMSTATE>;

    public sealed class FOC_FiniteStateMachine : FOComponent
    {
        private const int DEFAULT_STATE_COUNT = (int)ACTIONFSMSTATE.NONE;
        private readonly Dictionary<ACTIONFSMSTATE, bool> m_actTriggers = new Dictionary<ACTIONFSMSTATE, bool>(DEFAULT_STATE_COUNT, new ACTIONFSMSTATEComparer());
        private bool m_isAnyTriggerSet;
        public ACTIONFSMSTATE CurrentAction => ActionFSM?.StateKey ?? ACTIONFSMSTATE.IDLE;
        public FieldObjectActionFSM ActionFSM { get; private set; }

        public override void Initialize()
        {
            ActionFSM = new FieldObjectActionFSM(Owner, DEFAULT_STATE_COUNT, new ACTIONFSMSTATEComparer());

            ResetTriggers();
            SetStateTrigger(ACTIONFSMSTATE.IDLE);
        }

        public override void Update(float dt)
        {
            UpdateStateTransition();
            ActionFSM.Update(dt);
        }

        public override void LateUpdate(float dt)
        {
        }

        protected override void OnDispose()
        {

        }

        public void SetStateTrigger(ACTIONFSMSTATE state)
        {
            m_actTriggers[state] = true;
            m_isAnyTriggerSet = true;
        }

        public void UpdateStateTransition()
        {
            if (!m_isAnyTriggerSet) {
                return;
            }

            var nextState = ActionFSMRule.RequestStateTransition(CurrentAction, m_actTriggers);
            if (nextState != ACTIONFSMSTATE.NONE) {
                ActionFSM.ChangeState(nextState);
            }
            ResetTriggers();
        }

        public void ResetTriggers()
        {
            foreach (var key in ActionFSMRule.TransitionKeys) {
                m_actTriggers[key] = false;
            }
            m_isAnyTriggerSet = false;
        }
    }
}
