using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace iShape.Spline {

    public static class SplineMath {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 GetPointFromLine(float2 pA, float2 pB, float k) {
            return pA + k * (pB - pA);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 GetPointFromCubic(float2 pA, float2 pB, float2 ab, float k) {
            float2 ppA = GetPointFromLine(pA, ab, k);
            float2 ppB = GetPointFromLine(ab, pB, k);

            float2 p = GetPointFromLine(ppA, ppB, k);

            return p;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 GetPointFromQuartic(float2 pA, float2 pB, float2 a, float2 b, float k) {
            float2 ppA = GetPointFromLine(pA, a, k);
            float2 ppAB = GetPointFromLine(a, b, k);
            float2 ppB = GetPointFromLine(b, pB, k);

            float2 pppA = GetPointFromLine(ppA, ppAB, k);
            float2 pppB = GetPointFromLine(ppAB, ppB, k);

            float2 p = GetPointFromLine(pppA, pppB, k);

            return p;
        }
    }

}
