
using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;

namespace iShape.Spline {

    public readonly struct Spline {
    
        private enum Type {
            line,
            cubic,
            quartic
        }
        
        private readonly Type type;
        private readonly float2 pointA;
        private readonly float2 pointB;
        private readonly float2 anchorA;
        private readonly float2 anchorB;
       
        public Spline(float2 pointA, float2 pointB) {
            this.type = Type.line;
            this.pointA = pointA;
            this.pointB = pointB;
            this.anchorA = float2.zero;
            this.anchorB = float2.zero;
        }
        
        public Spline(float2 pointA, float2 pointB, float2 anchor) {
            this.type = Type.cubic;
            this.pointA = pointA;
            this.pointB = pointB;
            this.anchorA = anchor;
            this.anchorB = float2.zero;
        }
        
        public Spline(float2 pointA, float2 pointB, float2 anchorA, float2 anchorB) {
            this.type = Type.quartic;
            this.pointA = pointA;
            this.pointB = pointB;
            this.anchorA = anchorA;
            this.anchorB = anchorB;
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
        
        public float2 Point(float k) {
            return type switch {
                Type.line => PointForLine(pointA, pointB, k),
                Type.cubic => PointForCubic(pointA, pointB, anchorA, k),
                Type.quartic => PointForQuartic(pointA, pointB, anchorA, anchorB, k),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        public NativeArray<float2> Points(float stepLength, Allocator allocator) {
            float length = Length(20);
            int n = (int)(length / stepLength + 0.5f);
            float s = 1.0f / n;
            float t = 0;
            var result = new NativeArray<float2>(n + 1, allocator);
            
            for (int i = 0; i < n; i++) {
                result[i] = Point(t);
                t += s;
            }
            
            result[n] = Point(t);

            return result;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 PointForLine(float2 pA, float2 pB, float k) {
            return pA + k * (pB - pA);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 PointForCubic(float2 pA, float2 pB, float2 ab, float k) {
            float2 ppA = PointForLine(pA, ab, k);
            float2 ppB = PointForLine(ab, pB, k);

            float2 p = PointForLine(ppA, ppB, k);

            return p;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 PointForQuartic(float2 pA, float2 pB, float2 a, float2 b, float k) {
            float2 ppA = PointForLine(pA, a, k);
            float2 ppAB = PointForLine(a, b, k);
            float2 ppB = PointForLine(b, pB, k);

            float2 pppA = PointForLine(ppA, ppAB, k);
            float2 pppB = PointForLine(ppAB, ppB, k);

            float2 p = PointForLine(pppA, pppB, k);

            return p;
        }
    }

}
