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
                result[i] = GetPoint(t);
                t += s;
            }
            
            result[count] = GetPoint(1);

            return result;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float2 GetPoint(float k) {
            return SplineMath.GetPointFromCubic(pointA, pointB, anchor, k);
        }
    }

}
