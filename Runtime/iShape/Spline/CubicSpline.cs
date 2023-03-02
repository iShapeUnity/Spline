using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;

namespace iShape.Spline {

    public readonly struct CubicSpline {

        private readonly float2 pointA;
        private readonly float2 pointB;
        private readonly float2 anchor;

        public CubicSpline(float2 pointA, float2 pointB, float2 anchor) {
            this.pointA = pointA;
            this.pointB = pointB;
            this.anchor = anchor;
        }

        public NativeArray<float2> GetPoints(int count, Allocator allocator) {
            var result = new NativeArray<float2>(count + 1, allocator);
            float s = 1f / count;
            float t = 0;
            for (int i = 0; i < count; i++) {
                result[i] = Point(t);
                t += s;
            }
            
            result[count] = Point(1);

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2 Point(float k) {
            return Spline.PointForCubic(pointA, pointB, anchor, k);
        }
        
        public float Length(int stepCount) {
            var prevPoint = Point(0f);

            float step = 1.0f / stepCount;
            float path = step;
            float length = 0f;

            for (int i = 0; i < stepCount; i++) {
                var nextPoint = Point(path);
                length += math.distance(nextPoint, prevPoint);

                prevPoint = nextPoint;
                path += step;
            }

            return length;
        }
    }

}
