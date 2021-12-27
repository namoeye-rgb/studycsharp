using System;
using System.Collections.Generic;
using System.Text;

namespace UtilLib
{
    public class Vec2
    {
        public static Vec2 zero => new Vec2(0, 0);
        public static Vec2 up => new Vec2(0, 1);
        public static Vec2 right => new Vec2(1, 0);
        public static Vec2 one => new Vec2(1, 1);

        public double X { get; }
        public double Y { get; }

        public Vec2 YX => new Vec2(Y, X);
        public Vec3 OXY => new Vec3(0, X, Y);
        public Vec3 XOY => new Vec3(X, 0, Y);
        public Vec3 XYO => new Vec3(X, Y, 0);

        public double this[int index] {
            get {
                switch (index) {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public Vec2 Set(int index, double v)
        {
            switch (index) {
                case 0: return SetX(v);
                case 1: return SetY(v);
                default: throw new ArgumentException("index must be 0 or 1");
            }
        }

        public Vec2 SetX(double v)
        {
            return new Vec2(v, Y);
        }

        public Vec2 SetY(double v)
        {
            return new Vec2(X, v);
        }

        public Vec2(int px, int py)
        {
            X = px;
            Y = py;
        }

        public Vec2(float px, float py)
        {
            X = px;
            Y = py;
        }

        public Vec2(double px, double py)
        {
            X = px;
            Y = py;
        }

        public Vec2(Vec2 other) : this(other.X, other.Y)
        {
        }

        public static Vec2 operator -(Vec2 self)
        {
            return new Vec2(-self.X, -self.Y);
        }

        public static Vec2 operator +(Vec2 target, Vec2 source)
        {
            return new Vec2(target.X + source.X, target.Y + source.Y);
        }

        public static Vec2 operator -(Vec2 target, Vec2 source)
        {
            return new Vec2(target.X - source.X, target.Y - source.Y);
        }

        public static Vec2 operator *(Vec2 target, float source)
        {
            return new Vec2(target.X * source, target.Y * source);
        }

        public static Vec2 operator /(Vec2 target, float source)
        {
            return new Vec2(target.X / source, target.Y / source);
        }

        public static Vec2 operator *(Vec2 target, double source)
        {
            return new Vec2(target.X * source, target.Y * source);
        }

        public static Vec2 operator /(Vec2 target, double source)
        {
            return new Vec2(target.X / source, target.Y / source);
        }

        public static bool operator ==(Vec2 target, Vec2 source)
        {
            return Math.Abs(target.X - source.X) < double.Epsilon && Math.Abs(target.Y - source.Y) < double.Epsilon;
        }

        public static bool operator !=(Vec2 target, Vec2 source)
        {
            return Math.Abs(target.X - source.X) > double.Epsilon || Math.Abs(target.Y - source.Y) > double.Epsilon;
        }

        public static double GetRadian(Vec2 from, Vec2 to)
        {
            double radian = Math.Atan2(to.Y - from.Y, to.X - from.X);// - Math.Atan2(from.y, from.x);
            return radian;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vec2 target) {
                return Math.Abs(X - target.X) <= double.Epsilon
                    && Math.Abs(Y - target.Y) <= double.Epsilon;
            }
            return false;
        }

        public override string ToString()
        {
            return new StringBuilder().Append(X).Append(", ").Append(Y).ToString();
        }

        public string ToString(string fmt)
        {
            return new StringBuilder().AppendFormat(fmt, X, Y).ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public double magnitude => Math.Sqrt(X * X + Y * Y);

        public double sqrMagnitude => X * X + Y * Y;

        public Vec2 Normalize()
        {
            double length = magnitude;
            if (length > 0) {
                return new Vec2(X / length, Y / length);
            }
            return zero;
        }

        public Vec2 Translate(float x, float y)
        {
            return new Vec2(X + x, Y + y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public Vec2 RotateAngle(double angle)
        {
            double radian = angle * (Math.PI / 180);
            return Rotate(radian);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        ///  Y
        ///  * 
        ///  *  \
        ///  *   \
        ///  *    \
        ///  *    _\|
        ///  *     
        ///  O * * * * * * * X
        /// </remarks>
        /// <param name="radian"></param>
        /// <returns></returns>
        public Vec2 Rotate(double radian)
        {
            return Rotate(Math.Sin(radian), Math.Cos(radian));
        }

        /// <summary> </summary>
        /// <remarks>
        ///  Y
        ///  * 
        ///  *  \
        ///  *   \
        ///  *    \
        ///  *    _\|
        ///  *     
        ///  O * * * * * * * X
        /// </remarks>
        /// <param name="sin"></param>
        /// <param name="cos"></param>
        /// <returns></returns>
        public Vec2 Rotate(double sin, double cos)
        {
            return new Vec2(
                cos * X + sin * Y,
                -sin * X + cos * Y
            );
        }

        /// <summary>
        ///     angle 만큼 반시계 방향으로 돌립니다. (양각)
        /// </summary>
        /// <remarks>
        ///  Y
        ///  *   __
        ///  *  |\
        ///  *    \
        ///  *     \
        ///  *      \
        ///  *
        ///  O * * * * * * * X
        /// </remarks>
        /// <param name="angle"></param>
        /// <returns></returns>
        public Vec2 RotateAngleAntiClockwise(double angle)
        {
            double radian = angle * (Math.PI / 180);
            return RotateAntiClockwise(radian);
        }

        /// <summary>
        ///     radian 만큼 반시계 방향으로 돌립니다. (양각)
        /// </summary>
        /// <remarks>
        ///  Y
        ///  *   __
        ///  *  |\
        ///  *    \
        ///  *     \
        ///  *      \
        ///  *
        ///  O * * * * * * * X
        /// </remarks>
        /// <paramref name="radian"/>
        /// <returns></returns>
        public Vec2 RotateAntiClockwise(double radian)
        {
            return RotateAntiClockwise(Math.Sin(radian), Math.Cos(radian));
        }

        /// <summary>
        ///     sin, cos 값을 이용하여 반시계 방향으로 회전합니다.
        /// </summary>
        /// <remarks>
        ///  Y
        ///  *   __
        ///  *  |\
        ///  *    \
        ///  *     \
        ///  *      \
        ///  *
        ///  O * * * * * * * X
        /// </remarks>
        /// <param name="sin"></param>
        /// <param name="cos"></param>
        /// <returns></returns>
        public Vec2 RotateAntiClockwise(double sin, double cos)
        {
            return new Vec2(
                cos * X + -sin * Y,
                sin * X + cos * Y
            );
        }

        public double DotProduct(Vec2 vec)
        {
            return X * vec.X + Y * vec.Y;
        }

        public double DistanceTo(Vec2 target)
        {
            return (this - target).magnitude;
        }

        public double SqrDistanceTo(Vec2 target)
        {
            return (this - target).sqrMagnitude;
        }

        public double PositiveAtan2 {
            get {
                double angle = MATH.atan2Approximation(Y, X) * MATH.Rad2Deg;
                if (angle < 0) {
                    angle += 360;
                }

                return angle;
            }
        }

        /// <summary>
        ///     벡터 a, b를 3차원 스칼라가 0인 3차원 벡터로 바꾸어 외적했을 때 결과 벡터의 3차원 크기를 구합니다.
        /// </summary>
        /// <remarks>
        ///     이건 사실 [ a0, a1 ] 행렬의 행렬식(determinant)에 해당합니다.
        ///              [ b0, b1 ] 
        /// </remarks>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double CrossProduct(Vec2 a, Vec2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }
    }

    public class Vec3
    {
        public static readonly Vec3 zero = new Vec3(0, 0, 0);
        public static readonly Vec3 up = new Vec3(0, 1, 0);
        public static readonly Vec3 right = new Vec3(1, 0, 0);
        public static readonly Vec3 one = new Vec3(1, 1, 1);

        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public Vec2 XY => new Vec2(X, Y);
        public Vec2 XZ => new Vec2(X, Z);
        public Vec2 YX => new Vec2(Y, X);
        public Vec2 YZ => new Vec2(Y, Z);
        public Vec2 ZX => new Vec2(Z, X);
        public Vec2 ZY => new Vec2(Z, Y);

        public double this[int index] {
            get {
                switch (index) {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public Vec3 Set(int i, double v)
        {
            switch (i) {
                case 0: return SetX(v);
                case 1: return SetY(v);
                case 2: return SetZ(v);
                default: throw new IndexOutOfRangeException();
            }
        }

        public Vec3 SetX(double v)
        {
            return new Vec3(v, Y, Z);
        }

        public Vec3 SetY(double v)
        {
            return new Vec3(X, v, Z);
        }

        public Vec3 SetZ(double v)
        {
            return new Vec3(X, Y, v);
        }

        public Vec3(int px, int py, int pz)
        {
            X = px;
            Y = py;
            Z = pz;
        }

        public Vec3(float px, float py, float pz)
        {
            X = px;
            Y = py;
            Z = pz;
        }

        public Vec3(double px, double py, double pz)
        {
            X = px;
            Y = py;
            Z = pz;
        }

        public Vec3(Vec3 other) : this(other.X, other.Y, other.Z)
        {

        }

        public static Vec3 operator -(Vec3 self)
        {
            return new Vec3(-self.X, -self.Y, -self.Z);
        }

        public static Vec3 operator +(Vec3 target, Vec3 source)
        {
            return new Vec3(target.X + source.X, target.Y + source.Y, target.Z + source.Z);
        }

        public static Vec3 operator -(Vec3 target, Vec3 source)
        {
            return new Vec3(target.X - source.X, target.Y - source.Y, target.Z + source.Z);
        }

        public static Vec3 operator *(Vec3 target, double source)
        {
            return new Vec3(target.X * source, target.Y * source, target.Z * source);
        }

        public static Vec3 operator /(Vec3 target, double source)
        {
            return new Vec3(target.X / source, target.Y / source, target.Z / source);
        }

        public static bool operator ==(Vec3 target, Vec3 source)
        {
            return Math.Abs(target.X - source.X) < double.Epsilon
                && Math.Abs(target.Y - source.Y) < double.Epsilon
                && Math.Abs(target.Z - source.Z) < double.Epsilon;
        }

        public static bool operator !=(Vec3 target, Vec3 source)
        {
            return Math.Abs(target.X - source.X) > double.Epsilon
                || Math.Abs(target.Y - source.Y) > double.Epsilon
                || Math.Abs(target.Z - source.Z) > double.Epsilon;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vec3 target) {
                return Math.Abs(X - target.X) <= double.Epsilon
                    && Math.Abs(Y - target.Y) <= double.Epsilon
                    && Math.Abs(Z - target.Z) <= double.Epsilon;
            }
            return false;
        }

        public override string ToString()
        {
            return new StringBuilder().Append(X).Append(", ").Append(Y).Append(", ").Append(Z).ToString();
        }

        public override int GetHashCode()
        {
            return (X.GetHashCode() * 37) ^ (Y.GetHashCode() * 299) ^ (Z.GetHashCode() * 967);
        }

        public double magnitude => Math.Sqrt(X * X + Y * Y + Z * Z);

        public double sqrMagnitude => X * X + Y * Y + Z * Z;

        public Vec3 Normalize()
        {
            double length = magnitude;
            if (length > 0) {
                return new Vec3(X / length,
                                Y / length,
                                Z / length);
            }
            return zero;
        }

        public double DotProduct(Vec3 vec)
        {
            return X * vec.X + Y * vec.Y + Z * vec.Z;
        }

        public Vec3 CrossProduct(Vec3 o)
        {
            double cx = Y * o.Z - Z * o.Y;
            double cy = Z * o.X - X * o.Z;
            double cz = X * o.Y - Y * o.X;
            return new Vec3(cx, cy, cz);
        }

        public double DistanceTo(Vec3 target)
        {
            return (this - target).magnitude;
        }

        public float[] ToFloatArray()
        {
            return new float[3] { (float)X, (float)Y, (float)Z };
        }

        public double[] ToDoubleArray()
        {
            return new double[3] { X, Y, Z };
        }
    }

    public static class MATH
    {
        static MATH()
        {
            int count = 720;
            s_radialVectors = new Vec2[720];
            for (int i = 0; i < count; ++i) {
                s_radialVectors[i] = Vec2.right.Rotate(Math.PI * i / 360);
            }
        }
        public static Vec2[] s_radialVectors;

        public const double Deg2Rad = 0.0174532924;
        public const double Rad2Deg = 57.29578;

        public static double Clamp01(double value)
        {
            if (value < 0) {
                value = 0;
            } else if (value > 1.0f) {
                value = 1;
            }

            return value;
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value < min) {
                value = min;
            } else if (value > max) {
                value = max;
            }

            return value;
        }

        public static double Sin(double ratio, double min, double max)
        {
            return (Math.Sin(Math.PI * 2 * ratio) * 0.5 + 0.5) * (max - min) + min;
        }

        public static double Cos(double ratio, double min, double max)
        {
            return (Math.Cos(Math.PI * 2 * ratio) * 0.5 + 0.5) * (max - min) + min;
        }

        public static double GetSlerpRatio(double ratio)
        {
            double newRatio = (1 - Math.Cos(ratio * Math.PI)) * 0.5;
            return newRatio;
        }

        public static double GetPowRatio(double ratio, bool inverse = false)
        {
            double newRatio = Math.Pow(ratio, 2);
            if (inverse) {
                if (newRatio > 0) {
                    newRatio = Math.Sqrt(ratio);
                }
            }

            newRatio = (1 - Math.Cos(newRatio * Math.PI)) * 0.5;

            return newRatio;
        }

        public static double GetDistance(Vec2 pos1, Vec2 pos2)
        {
            return Math.Sqrt(Math.Pow(pos1.X - pos2.X, 2) + Math.Pow(pos1.Y - pos2.Y, 2));
        }

        public static bool IsInRange(Vec2 org, Vec2 target, double range)
        {
            double dist = (org - target).magnitude;

            if (range > dist) {
                return true;
            }

            return false;
        }

        public static bool IntersectSegment(Vec2 circle, double radius, Vec2 start, Vec2 end)
        {
            double dx, dy, A, B, C, det, t;

            dx = end.X - start.X;
            dy = end.Y - start.Y;

            A = dx * dx + dy * dy;
            B = 2 * (dx * (start.X - circle.X) + dy * (start.Y - circle.Y));
            C = (start.X - circle.X) * (start.X - circle.X) + (start.Y - circle.Y) * (start.Y - circle.Y) - radius * radius;

            det = B * B - 4 * A * C;

            if (Math.Abs(det) < double.Epsilon) {
                t = -B / (2 * A);
                if (t >= 0 && t <= 1) {
                    return true;
                }
            }

            if (det > 0) {
                t = (-B + Math.Sqrt(det)) / (2 * A);
                if (t >= 0 && t <= 1) {
                    return true;
                }

                t = (-B - Math.Sqrt(det)) / (2 * A);
                if (t >= 0 && t <= 1) {
                    return true;
                }
            }
            return false;
        }

        public static Vec2 AngleToVec(double degree)
        {
            double radian = degree * Deg2Rad;
            return new Vec2(Math.Cos(radian), Math.Sin(radian));
        }

        public static double VecToDegree(Vec2 vec)
        {
            return Math.Atan2(vec.Y, vec.X) * Rad2Deg;
        }

        public static int VecToAngleInt(Vec2 vec)
        {
            return VecToAngleInt(vec.X, vec.Y);
        }

        public static int VecToAngleInt(double x, double y)
        {
            return (int)(Math.Atan2(y, x) * Rad2Deg);
        }

        public static double AngleToRadian(double angle_)
        {
            return Deg2Rad * angle_;
        }

        public static double RadianToAngle(double rad_)
        {
            return rad_ * Rad2Deg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns>[-180, 180] alignmented </returns> // 반시계방향이 양수(+)
        public static double AngleWithTwoDirection(Vec2 v1, Vec2 v2)
        {
            var cross = v1.X * v2.Y - v1.Y * v2.X;
            var dot = v1.X * v2.X + v1.Y * v2.Y;
            double degree = atan2Approximation(cross, dot) * Rad2Deg; /*Rad2Deg*/
            return degree;
        }

        public static double atan2Approximation(double y, double x) // http://http.developer.nvidia.com/Cg/atan2.html
        {
            double t0, t1,/*, t2*/ t3, t4;
            t3 = Math.Abs(x);
            t1 = Math.Abs(y);
            t0 = Math.Max(t3, t1);
            t1 = Math.Min(t3, t1);
            t3 = t1 * (1f / t0);
            t4 = t3 * t3;
            t0 = -0.013480470;
            t0 = t0 * t4 + 0.057477314;
            t0 = t0 * t4 - 0.121239071;
            t0 = t0 * t4 + 0.195635925;
            t0 = t0 * t4 - 0.332994597;
            t0 = t0 * t4 + 0.999995630;
            t3 = t0 * t3;
            t3 = (Math.Abs(y) > Math.Abs(x)) ? (Math.PI / 2) - t3 : t3;
            t3 = (x < 0) ? 3.141592654f - t3 : t3;
            t3 = (y < 0) ? -t3 : t3;
            return t3;
        }

        private static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            Random rnd = new Random();
            while (n > 1) {
                int k = rnd.Next(0, n) % n;
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static Vec2[] PlaceUniformlySizedCirclesRandomlyOnRadialRange(int count, double radius, double minDist, double maxDist)
        {
            var list = new Vec2[count];

            var candidates = new List<Vec2>();
            for (double dist = minDist; dist < maxDist; dist += radius * 2) {
                double theta = Math.Asin(radius / dist);
                for (double angle = 0; angle < Math.PI * 2 - theta * 2 + 0.001; angle += 2 * theta) {
                    double x = Math.Cos(angle);
                    double y = Math.Sin(angle);
                    candidates.Add(new Vec2(x, y) * dist);
                }
            }

            var indicies = new int[count];
            Shuffle(candidates);

            int pickCount = count < candidates.Count ? count : candidates.Count;
            candidates.CopyTo(0, list, 0, pickCount);

            // Not enough candidates were given. return zero vector for the last.
            for (int i = candidates.Count; i < count; ++i) {
                list[i] = Vec2.zero;
            }

            return list;
        }

        public static bool IsInRadialRange(this Vec2 self, Vec2 other, double range)
        {
            return (self - other).sqrMagnitude <= (range * range);
        }

        /// <summary> 호 위의 점들을 구합니다. </summary>
        /// <param name="angularDistance"> 각점들 사이의 각도거리차. 방향은 반시계 방향입니다. (x양축에서 y양축으로) 단위는 도degree.</param>
        /// <param name="beginAngle"> 호의 시작지점의 각도. x축으로 부터 잽니다. 단위는 도degree. <paramref name="endAngle"/>보다 클 수 없습니다. <paramref name="beginAngle"/></param>
        /// <param name="endAngle"> 호의 끝지점의 각도. x축으로 부터 잽니다. 단위는 도degree. </param>
        /// <param name="vs"> 점이 쓰여질 버퍼. 점의 갯수(정밀도)도 결정합니다.</param>
        /// <param name="radius"> 호의 반지름 </param>
        /// <returns></returns>
        public static void PopulateArcEdgePoints(IList<Vec2> vs, double radius, double beginAngle, double endAngle, double angularDistance)
        {
            if (angularDistance > 0) {
                if (beginAngle > endAngle) {
                    throw new ArgumentException($"방향이 시계방향일 때 beginAngle이 endAngle보다 클 수 없습니다. (beginAngle : {beginAngle}, endangle {endAngle})");
                }
            }

            if (angularDistance < 0) {
                if (beginAngle < endAngle) {
                    throw new ArgumentException($"방향이 반시계 방향일 때 endAngle이 beginAngle보다 클 수 없습니다. (beginAngle : {beginAngle}, endangle {endAngle})");
                }
            }

            if (radius <= 0) {
                throw new ArgumentException("radius가 0보다 작거나 같을 수 없습니다.");
            }

            if (vs == null) {
                throw new NullReferenceException("vs가 'null' 입니다.");
            }

            if (angularDistance > 0) {
                double a = beginAngle;
                while (a < endAngle) {
                    vs.Add(Vec2.right.RotateAngleAntiClockwise(a) * radius);
                    a += angularDistance;
                }
            } else if (angularDistance < 0) {
                double a = beginAngle;
                while (a > endAngle) {
                    vs.Add(Vec2.right.RotateAngleAntiClockwise(a) * radius);
                    a += angularDistance;
                }
            }


        }

        /// <summary> </summary>
        /// <param name="n"> 0 based index </param>
        /// <returns></returns>
        private static uint CircleCount(uint n)
        {
            return 3U * n * n + 3U * n + 1U;
        }

        /// <summary> 주어진 원의 반지름과 위치 인덱스를 이용해 6각형 원 배치 위치를 구합니다. </summary>
        /// <param name="radius"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <remarks>
        ///     0일 때 가장 중앙의 원,
        ///     1~6일 때 0의 원에 인접한 원들을 반시계방향으로 반환,
        ///     7~18일 때 1~6에서 배치되었던 원들에 각각 인접한 원을 반환,
        ///     ...
        /// </remarks>
        //public static PolarCoordinate GetHexagonalCirclePackLocation(double radius, uint index)
        //{
        //    if (index == 0)
        //        return PolarCoordinate.Zero;
        //    uint n = 0;
        //    while (CircleCount(n) <= index)
        //    {
        //        ++n;
        //    }
        //    uint j = index - CircleCount(n - 1);
        //    double deltaAngle = Math.PI / 3 / n;
        //    return new PolarCoordinate(2 * radius * n, deltaAngle * j);
        //}

        //public static Vec2 ToVec2(this PolarCoordinate pc)
        //{
        //    return new Vec2(Math.Cos(pc.theta), Math.Sin(pc.theta)) * pc.r;
        //}

        public static string ToChar(this Vec2 self)
        {
            double a = self.PositiveAtan2;
            const double step = 45;
            double curr = 22.5;
            if (a < curr) {
                return "→";
            }

            curr += step;
            if (a < curr) {
                return "↗";
            }

            curr += step;
            if (a < curr) {
                return "↑";
            }

            curr += step;
            if (a < curr) {
                return "↖";
            }

            curr += step;
            if (a < curr) {
                return "←";
            }

            curr += step;
            if (a < curr) {
                return "↙";
            }

            curr += step;
            if (a < curr) {
                return "↓";
            }

            curr += step;
            if (a < curr) {
                return "↘";
            }

            curr += step;
            if (a < curr) {
                return "→";
            }

            return ".";
        }
    }
}
