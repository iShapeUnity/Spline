using Unity.Collections;
using Unity.Mathematics;

namespace iShape.Spline {
    
    public readonly struct Curve {
        
        public readonly Spline[] splines;
        public readonly float length;
        private readonly Range[] ranges;
        private readonly bool isClosed;

        public Curve(NativeArray<Anchor> anchors, bool isClosed, int stepCount = 20) {
            this.isClosed = isClosed;
            int n = anchors.Length;
            
            int m = isClosed ? n : n - 1;         
            splines = new Spline[m];
            var lengths = new float[m];
            
            float l = 0f;

            for (int i = 0; i < m; i++) {
                int j = (i + 1) % n;
                var spline = SplineBuilder.Create(anchors[i], anchors[j]);
                float dl = spline.Length(stepCount);
                splines[i] = spline;
                lengths[i] = dl;
                l += dl;
            }

            length = l;
            ranges = new Range[m];
            
            float w = 0f;
            
            for (int i = 0; i < m; i++) {
                float dl = lengths[i];
                
                float dw = dl / l;

                ranges[i] = new Range {
                    start = w,
                    weight = dw
                };

                w += dw;
            }
        }

        public NativeArray<float2> GetPoints(float step, float2 pos, Allocator allocator) {
            int n = splines.Length;
            
            int m = (int)(length / step + 0.5f);
            var result = new NativeArray<float2>(m + 1, allocator);

            float t = 0;
            float s = 1f / m;
            int i = 0;
            var sp = splines[0];
            var r = ranges[0];
            for (int j = 0; j < m; j++) {
                while (t > r.end && i < n) {
                    i += 1;
                    sp = splines[i];
                    r = ranges[i];
                }

                float k = (t - r.start) / r.weight;
                var p = sp.Point(k) + pos;
                result[j] = p;
                t += s;
            }

            result[m] = sp.Point(1) + pos;

            return result;
        }

        public NativeArray<float2> GetPoints(float start, float end, float dw, float2 pos, Allocator allocator) {
            float w;
            if (isClosed) {
                start = start.Normalize();
                end = end.Normalize();
                float delta = end - start;
                if (start <= end) {
                    w = delta;
                } else {
                    w = 1 + delta;
                }
            } else {
                start = math.min(1, math.max(0, start));
                end = math.min(1, math.max(0, end));
                if (start > end) {
                    (start, end) = (end, start);
                }

                w = end - start;
            }

            int i = ranges.FindIndex(start);
            var sp = splines[i];

            if (w < 0.005f * dw) {
                var single = new NativeArray<float2>(1, allocator);
                single[0] = GetPoint(start) + pos;
                return single;
            }

            int count = (int)(w / dw + 0.5f);
            float s = w / count;
            
            var result = new NativeArray<float2>(count + 1, allocator);

            var r = ranges[i];
            var t = start;
            for (int j = 0; j <= count; j++) {
                float k = (t - r.start) / r.weight;
                var p = sp.Point(k) + pos;
                result[j] = p;
                t += s;
                while (t > r.end) {
                    if (t >= 1) {
                        if (isClosed) {
                            t = t.Normalize();
                            i = 0;
                        } else {
                            t = 1f;
                            break;
                        }
                    } else {
                        i += 1;    
                    }
                    r = ranges[i];
                    sp = splines[i];
                }
            }

            return result;
        }

        public float2 GetPoint(float weight) {
            int i = ranges.FindIndex(weight);
            var r = ranges[i];
            var sp = splines[i];
            float k = (weight - r.start) / r.weight;
            return sp.Point(k);
        }
    }

}