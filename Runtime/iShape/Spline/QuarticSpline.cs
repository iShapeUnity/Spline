using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;

namespace iShape.Spline {

    public readonly struct QuarticSpline {
    
        private readonly float2 pointA;
        private readonly float2 pointB;
        private readonly float2 anchorA;
        private readonly float2 anchorB;

        public QuarticSpline(float2 pointA, float2 pointB, float2 anchorA, float2 anchorB) {
            this.pointA = pointA;
            this.pointB = pointB;
            this.anchorA = anchorA;
            this.anchorB = anchorB;
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
            return Spline.PointForQuartic(pointA, pointB, anchorA, anchorB, k);
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
