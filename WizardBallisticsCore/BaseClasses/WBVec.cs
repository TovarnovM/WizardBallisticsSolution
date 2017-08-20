using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardBallistics.Core {
    /// <summary>Vector implementation. This is thin wrapper over 1D array</summary>
    public struct WBVec {
        public double[] v;

        /// <summary>Gets number of components in a vector</summary>
        /// <exception cref="NullReferenceException">Thrown when vector constructed by default constructor and not assigned yet</exception>
        public int Length {
            get { return v == null ? 0 : v.Length; }
        }

        /// <summary>Constructs vector from array of arguments</summary>
        /// <param name="elts">Elements of array</param>
        /// <example>This creates vector with three elements -1,0,1. New
        /// array is allocated for this vector:
        /// <code>Vector v = new Vector(-1,0,1)</code>
        /// Next example creates vector that wraps specified array. Please note
        /// that no array copying is made thus changing of source array with change vector elements.
        /// <code>
        /// double[] arr = new double[] { -1,0,1 };
        /// Vector v = new Vector(arr);
        /// </code>
        /// </example>
        public WBVec(params double[] elts) {
            v = elts ?? throw new ArgumentNullException("elts");
        }

        /// <summary>Constructs vector of specified length filled with zeros</summary>
        /// <param name="n">Length of vector</param>
        /// <returns>Constructed vector</returns>
        public static WBVec Zeros(int n) {
            WBVec v = new WBVec() {
                v = new double[n]
            };
            return v;
        }

        /// <summary>Clones specified vector</summary>
        /// <param name="v">Vector to clone</param>
        /// <returns>Copy of vector passes as parameter</returns>
        public WBVec Clone() {
            return v == null ? new WBVec() : new WBVec((double[])v.Clone());
        }

        /// <summary>
        /// Copies vector to double[] array 
        /// </summary>
        /// <returns></returns>
        public double[] ToArray() {
            return (double[])v.Clone();
        }

        /// <summary>Copies content of one vector to another. Vectors must have same length.</summary>
        /// <param name="src">Source vector</param>
        /// <param name="dst">Vector to copy results</param>
        public static void Copy(WBVec src, WBVec dst) {
            if (src.v == null)
                throw new ArgumentNullException("src");
            if (dst.v == null)
                throw new ArgumentNullException("dst");
            int n = src.v.Length;
            if (dst.v.Length != n)
                dst.v = new double[n];
            Array.Copy(src.v, dst.v, n);
        }



        /// <summary>Returns Euclidean norm of difference between two vectors. 
        /// </summary>
        /// <param name="v1">First vector</param>
        /// <param name="v2">Second vector</param>
        /// <returns>Euclidean norm of vector's difference</returns>
        public static double GetEuclideanNorm(WBVec v1, WBVec v2) {
            double[] av1 = v1.v;
            double[] av2 = v2.v;
            if (av1 == null)
                throw new ArgumentNullException("v1");
            if (av2 == null)
                throw new ArgumentNullException("v2");
            if (av1.Length != av2.Length)
                throw new ArgumentException("Vector lenghtes do not match");
            double norm = 0;
            for (int i = 0; i < av1.Length; i++)
                norm += (av1[i] - av2[i]) * (av1[i] - av2[i]);
            return Math.Sqrt(norm);
        }

        /// <summary>Returns L-infinity norm of difference between two vectors. 
        /// </summary>
        /// <param name="v1">First vector</param>
        /// <param name="v2">Second vector</param>
        /// <returns>L-infinity norm of vector's difference</returns>
        public static double GetLInfinityNorm(WBVec v1, WBVec v2) {
            double[] av1 = v1.v;
            double[] av2 = v2.v;
            if (av1 == null)
                throw new ArgumentNullException("v1");
            if (av2 == null)
                throw new ArgumentNullException("v2");
            if (av1.Length != av2.Length)
                throw new ArgumentException("Vector lenghtes do not match");
            double norm = 0;
            for (int i = 0; i < av1.Length; i++)
                norm = Math.Max(norm, Math.Abs(av1[i] - av2[i]));
            return norm;
        }

        /// <summary>Performs linear intepolation between two vectors at specified point</summary>
        /// <param name="t">Point of intepolation</param>
        /// <param name="t0">First time point</param>
        /// <param name="v0">Vector at first time point</param>
        /// <param name="t1">Second time point</param>
        /// <param name="v1">Vector at second time point</param>
        /// <returns>Intepolated vector value at point <paramref name="t"/></returns>
        public static WBVec Lerp(double t, double t0, WBVec v0, double t1, WBVec v1) {
            return (v0 * (t1 - t) + v1 * (t - t0)) / (t1 - t0);
        }

        /// <summary>
        /// Fast swap
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        public static void Swap(ref WBVec v1, ref WBVec v2) {
            var vtemp = v1.v;
            v1.v = v2.v;
            v2.v = vtemp;
        }

        /// <summary>Gets or sets vector element at specified index</summary>
        /// <exception cref="NullReferenceException">Thrown when vector is not initialized</exception>
        /// <exception cref="IndexOutOfRangeException">Throws when <paramref name="idx"/> is out of range</exception>
        /// <param name="idx">Index of element</param>
        /// <returns>Value of vector element at specified index</returns>
        public double this[int idx] {
            get { return v[idx- StartIndex]; }
            set { v[idx- StartIndex] = value; }
        }

        public static int StartIndex = 1;

        /// <summary>
        /// Получить/назначть часть вектора, начиная с [fromInd-элекента и до toIndex-элемента] включительно 
        /// </summary>
        /// <param name="fromInd">ОТ (начиная с StartIndex) индекс</param>
        /// <param name="toIndex">ДО ()</param>
        /// <returns></returns>
        public WBVec this[int fromInd, int toIndex] {
            get {
                int length = toIndex - fromInd + 1;
                var res = WBVec.Zeros(length);
                Array.Copy(this.v, fromInd- StartIndex, res.v, 0, length);
                return res;
            }
            set {
                Array.Copy(value.v, 0, this.v, fromInd- StartIndex, toIndex - fromInd + 1);
            }

        }

        /// <summary>Performs conversion of vector to array</summary>
        /// <param name="v">Vector to be converted</param>
        /// <returns>Array with contents of vector</returns>
        /// <remarks>This conversions doesn't perform array copy. In fact in returns reference
        /// to the same data</remarks>
        public static implicit operator double[] (WBVec v) {
            return v.v;
        }

        /// <summary>Performs conversion of 1d vector to</summary>
        /// <param name="v">Vector to be converted</param>
        /// <returns>Scalar value with first component of vector</returns>
        /// <exception cref="InvalidOperationException">Thrown when vector length is not one</exception>
        public static implicit operator double(WBVec v) {
            double[] av = v;
            if (av == null)
                throw new ArgumentNullException("v");
            if (av.Length != 1)
                throw new InvalidOperationException("Cannot convert multi-element vector to scalar");
            return av[0];
        }

        /// <summary>Performs conversion of array to vector</summary>
        /// <param name="v">Array to be represented by vector</param>
        /// <returns>Vector that wraps specified array</returns>
        public static implicit operator WBVec(double[] v) {
            return new WBVec(v);
        }

        /// <summary>Performs conversion of scalar to vector with length 1</summary>
        /// <param name="v">Double precision vector</param>
        /// <returns>Vector that wraps array with 1 element</returns>
        public static implicit operator WBVec(double v) {
            return new WBVec(v);
        }

        /// <summary>Adds vector <paramref name="v1"/> multiplied by <paramref name="factor"/> to this object.</summary>
        /// <param name="v1">Vector to add</param>
        /// <param name="factor">Multiplication factor</param>
        public void MulAdd(WBVec v1, double factor) {
            double[] av1 = v1.v;
            if (av1 == null)
                throw new ArgumentNullException("v1");
            if (this.Length != av1.Length)
                throw new InvalidOperationException("Cannot add vectors of different length");

            for (int i = 0; i < v.Length; i++)
                v[i] = v[i] + factor * av1[i];
        }

        /// <summary>Sums two vectors. Vectors must have same length.</summary>
        /// <param name="v1">First vector</param>
        /// <param name="v2">Second vector</param>
        /// <returns>Sum of vectors</returns>
        public static WBVec operator +(WBVec v1, WBVec v2) {
            double[] av1 = v1;
            double[] av2 = v2;
            if (av1.Length != av2.Length)
                throw new InvalidOperationException("Cannot add vectors of different length");
            double[] result = new double[av1.Length];
            for (int i = 0; i < av1.Length; i++)
                result[i] = av1[i] + av2[i];
            return new WBVec(result);
        }

        /// <summary>Add a scalar to a vector.</summary>
        /// <param name="v">Vector</param>
        /// <param name="c">Scalar to add</param>
        /// <returns>Shifted vector</returns>
        public static WBVec operator +(WBVec v, double c) {
            double[] av = v;
            double[] result = new double[av.Length];
            for (int i = 0; i < av.Length; i++)
                result[i] = av[i] + c;
            return new WBVec(result);
        }


        /// <summary>Substracts first vector from second. Vectors must have same length</summary>
        /// <param name="v1">First vector</param>
        /// <param name="v2">Second vector</param>
        /// <returns>Difference of two vectors</returns>
        public static WBVec operator -(WBVec v1, WBVec v2) {
            double[] av1 = v1;
            double[] av2 = v2;
            if (av1.Length != av2.Length)
                throw new InvalidOperationException("Cannot subtract vectors of different length");
            double[] result = new double[av1.Length];
            for (int i = 0; i < av1.Length; i++)
                result[i] = av1[i] - av2[i];
            return new WBVec(result);
        }

        /// <summary>Multiplies a vector by a scalar (per component)</summary>
        /// <param name="v">Vector</param>
        /// <param name="a">Scalar</param>
        /// <returns>Vector with all components multiplied by scalar</returns>
        public static WBVec operator *(WBVec v, double a) {
            double[] av = v;
            double[] result = new double[av.Length];
            for (int i = 0; i < av.Length; i++)
                result[i] = a * av[i];
            return new WBVec(result);
        }

        /// <summary>Multiplies a vector by a scalar (per component)</summary>
        /// <param name="v">Vector</param>
        /// <param name="a">Scalar</param>
        /// <returns>Vector with all components multiplied by scalar</returns>
        public static WBVec operator *(double a, WBVec v) {
            double[] av = v;
            double[] result = new double[av.Length];
            for (int i = 0; i < av.Length; i++)
                result[i] = a * av[i];
            return new WBVec(result);
        }

        /// <summary>Performs scalar multiplication of two vectors</summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Result of scalar multiplication</returns>
        public static WBVec operator *(WBVec a, WBVec b) {
            if (a.Length != b.Length)
                throw new InvalidOperationException("Cannot multy vectors of different length");
            double[] res = WBVec.Zeros(a.Length);

            for (int i = 0; i < a.Length; i++) {
                res[i] = a[i] * b[i];
            }

            return res;
        }



        /// <summary>Divides vector by a scalar (per component)</summary>
        /// <param name="v">Vector</param>
        /// <param name="a">Scalar</param>
        /// <returns>Result of division</returns>
        public static WBVec operator /(WBVec v, double a) {
            double[] av = v;

            if (a == 0.0d) throw new DivideByZeroException("Cannot divide by zero");

            double[] result = new double[av.Length];
            for (int i = 0; i < av.Length; i++)
                result[i] = av[i] / a;
            return new WBVec(result);
        }

        /// <summary>Performs element-wise division of two vectors</summary>
        /// <param name="a">Numerator vector</param>
        /// <param name="b">Denominator vector</param>
        /// <returns>Result of scalar multiplication</returns>
        public static WBVec operator /(WBVec a, WBVec b) {
            if (a.Length != b.Length)
                throw new InvalidOperationException("Cannot element-wise divide vectors of different length");
            double[] res = WBVec.Zeros(a.Length);

            for (int i = 0; i < a.Length; i++) {
                if (b[i] == 0.0d) throw new DivideByZeroException("Cannot divide by zero");
                res[i] = a[i] / b[i];
            }

            return res;
        }

        /// <summary>
        /// Returns a vector each of whose elements is the maximal from the corresponding
        /// ones of argument vectors. Note that dimensions of the arguments must match.
        /// </summary>
        /// <param name="v1">First vector</param>
        /// <param name="v2">Second vector</param>
        /// <returns>vector v3 such that for each i = 0...dim(v1) v3[i] = max( v1[i], v2[i] )</returns>
        public static WBVec Max(WBVec v1, WBVec v2) {
            double[] av1 = v1.v;
            double[] av2 = v2.v;

            if (av1 == null)
                throw new ArgumentNullException("v1");
            if (av2 == null)
                throw new ArgumentNullException("v2");

            if (av1.Length != av2.Length)
                throw new ArgumentException("Vector lengths do not match");
            WBVec y = WBVec.Zeros(av1.Length);
            for (int i = 0; i < av1.Length; i++)
                y[i] = Math.Max(av1[i], av2[i]);

            return y;
        }

        /// <summary>
        /// Returns a vector whose elements are the absolute values of the given vector elements
        /// </summary>
        /// <param name="v">Vector to operate with</param>
        /// <returns>Vector v1 such that for each i = 0...dim(v) v1[i] = |v[i]|</returns>
        public WBVec Abs() {
            if (v == null)
                return new WBVec();

            int n = v.Length;
            WBVec y = WBVec.Zeros(n);
            for (int i = 0; i < n; i++)
                y[i] = Math.Abs(v[i]);
            return y;
        }

        /// <summary>Convers vector to string representation.</summary>
        /// <returns>String consists from vector components separated by comma.</returns>
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            if (v != null)
                for (int i = 0; i < v.Length; i++) {
                    if (i > 0) sb.Append(", ");
                    sb.Append(v[i].ToString("G6"));
                }
            sb.Append("]");
            return sb.ToString();
        }

        public string ToString(string mode = "G6") {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            if (v != null)
                for (int i = 0; i < v.Length; i++) {
                    if (i > 0)
                        sb.Append(", ");
                    sb.Append(v[i].ToString(mode));
                }
            sb.Append("]");
            return sb.ToString();
        }

        public override bool Equals(object obj) {
            if (obj is WBVec v2) {
                if (v2.Length != Length)
                    return false;
                var av2 = v2.v;
                for (var i = 0; i < v.Length; i++)
                    if (v[i] != av2[i])
                        return false;
                return true;
            } else
                return false;
        }

        public override int GetHashCode() {
            return v == null ? base.GetHashCode() : v.GetHashCode();
        }
    }

    //  public struct Vec3 {
    //      public double v1, v2, v3;
    //      public Vec3(double v1, double v2, double v3) {
    //          this.v1 = v1;
    //          this.v2 = v2;
    //          this.v3 = v3;
    //      }
    //      #region Operators
    //      public double this[int index] {
    //          get {
    //              switch (index) {
    //                  case 1:
    //                      return v1;
    //                  case 2:
    //                      return v2;
    //                  case 3:
    //                      return v3;
    //                  default:
    //                      throw new ArgumentOutOfRangeException();
    //              }
    //          }
    //          set {
    //              switch (index) {
    //                  case 1:
    //                      v1 = value;
    //                      break;
    //                  case 2:
    //                      v2 = value;
    //                      break;
    //                  case 3:
    //                      v3 = value;
    //                      break;
    //                  default:
    //                      throw new ArgumentOutOfRangeException();
    //              }
    //          }
    //      }
    //      /// <summary>
    ///// Returns the hashcode for this instance.
    ///// </summary>
    ///// <returns>A 32-bit signed integer hash code.</returns>
    //public override int GetHashCode() {
    //          return v1.GetHashCode() ^ v2.GetHashCode() ^ v3.GetHashCode();
    //      }
    //      /// <summary>
    //      /// Returns a value indicating whether this instance is equal to
    //      /// the specified object.
    //      /// </summary>
    //      /// <param name="obj">An object to compare to this instance.</param>
    //      /// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Vector3D"/> and has the same values as this instance; otherwise, <see langword="false"/>.</returns>
    //      public override bool Equals(object obj) {
    //          if (obj is Vec3 v) {
    //              return (v1 == v.v1) && (v2 == v.v2) && (v3 == v.v3);
    //          }
    //          return false;
    //      }

    //      public static bool AproxEq(Vec3 v, Vec3 u, double tol = 1E-6) {
    //          return Math.Abs(v.v1 - u.v1) < tol &&
    //              Math.Abs(v.v2 - u.v2) < tol &&
    //              Math.Abs(v.v3 - u.v3) < tol;
    //      }

    //      /// <summary>
    //      /// Returns a string representation of this object.
    //      /// </summary>
    //      /// <returns>A string representation of this object.</returns>
    //      public override string ToString() {
    //          return string.Format("({0}, {1}, {2})", v1, v2, v3);
    //      }
    //      #endregion

    //      #region Comparison Operators
    //      /// <summary>
    //      /// Tests whether two specified vectors are equal.
    //      /// </summary>
    //      /// <param name="u">The left-hand vector.</param>
    //      /// <param name="v">The right-hand vector.</param>
    //      /// <returns><see langword="true"/> if the two vectors are equal; otherwise, <see langword="false"/>.</returns>
    //      public static bool operator ==(Vec3 u, Vec3 v) {
    //          return ValueType.Equals(u, v);
    //      }
    //      /// <summary>
    //      /// Tests whether two specified vectors are not equal.
    //      /// </summary>
    //      /// <param name="u">The left-hand vector.</param>
    //      /// <param name="v">The right-hand vector.</param>
    //      /// <returns><see langword="true"/> if the two vectors are not equal; otherwise, <see langword="false"/>.</returns>
    //      public static bool operator !=(Vec3 u, Vec3 v) {
    //          return !ValueType.Equals(u, v);
    //      }
    //      /// <summary>
    //      /// Tests if a vector's components are greater than another vector's components.
    //      /// </summary>
    //      /// <param name="u">The left-hand vector.</param>
    //      /// <param name="v">The right-hand vector.</param>
    //      /// <returns><see langword="true"/> if the left-hand vector's components are greater than the right-hand vector's component; otherwise, <see langword="false"/>.</returns>
    //      public static bool operator >(Vec3 u, Vec3 v) {
    //          return (
    //              (u.v1 > v.v1) &&
    //              (u.v2 > v.v2) &&
    //              (u.v3 > v.v3));
    //      }
    //      /// <summary>
    //      /// Tests if a vector's components are smaller than another vector's components.
    //      /// </summary>
    //      /// <param name="u">The left-hand vector.</param>
    //      /// <param name="v">The right-hand vector.</param>
    //      /// <returns><see langword="true"/> if the left-hand vector's components are smaller than the right-hand vector's component; otherwise, <see langword="false"/>.</returns>
    //      public static bool operator <(Vec3 u, Vec3 v) {
    //          return (
    //              (u.v1 < v.v1) &&
    //              (u.v2 < v.v2) &&
    //              (u.v3 < v.v3));
    //      }
    //      /// <summary>
    //      /// Tests if a vector's components are greater or equal than another vector's components.
    //      /// </summary>
    //      /// <param name="u">The left-hand vector.</param>
    //      /// <param name="v">The right-hand vector.</param>
    //      /// <returns><see langword="true"/> if the left-hand vector's components are greater or equal than the right-hand vector's component; otherwise, <see langword="false"/>.</returns>
    //      public static bool operator >=(Vec3 u, Vec3 v) {
    //          return (
    //              (u.v1 >= v.v1) &&
    //              (u.v2 >= v.v2) &&
    //              (u.v3 >= v.v3));
    //      }
    //      /// <summary>
    //      /// Tests if a vector's components are smaller or equal than another vector's components.
    //      /// </summary>
    //      /// <param name="u">The left-hand vector.</param>
    //      /// <param name="v">The right-hand vector.</param>
    //      /// <returns><see langword="true"/> if the left-hand vector's components are smaller or equal than the right-hand vector's component; otherwise, <see langword="false"/>.</returns>
    //      public static bool operator <=(Vec3 u, Vec3 v) {
    //          return (
    //              (u.v1 <= v.v1) &&
    //              (u.v2 <= v.v2) &&
    //              (u.v3 <= v.v3));
    //      }
    //      #endregion

    //      #region Unary Operators
    //      /// <summary>
    //      /// Negates the values of the vector.
    //      /// </summary>
    //      /// <param name="v">A <see cref="Vector3D"/> instance.</param>
    //      /// <returns>A new <see cref="Vector3D"/> instance containing the negated values.</returns>
    //      public static Vec3 operator -(Vec3 v) {
    //          return new Vec3(-v.v1, -v.v2, -v.v3);
    //      }
    //      #endregion

    //      #region Binary Operators
    //      /// <summary>
    //      /// Adds two vectors.
    //      /// </summary>
    //      /// <param name="u">A <see cref="Vector3D"/> instance.</param>
    //      /// <param name="v">A <see cref="Vector3D"/> instance.</param>
    //      /// <returns>A new <see cref="Vector3D"/> instance containing the sum.</returns>
    //      public static Vec3 operator +(Vec3 u, Vec3 v) {
    //          return new Vec3(u.v1 + v.v1, u.v2 + v.v2, u.v3 + v.v3);
    //      }
    //      /// <summary>
    //      /// Adds a vector and a scalar.
    //      /// </summary>
    //      /// <param name="v">A <see cref="Vector3D"/> instance.</param>
    //      /// <param name="s">A scalar.</param>
    //      /// <returns>A new <see cref="Vector3D"/> instance containing the sum.</returns>
    //      public static Vec3 operator +(Vec3 v, double s) {
    //          return new Vec3(s + v.v1, s + v.v2, s + v.v3);
    //      }
    //      /// <summary>
    //      /// Adds a vector and a scalar.
    //      /// </summary>
    //      /// <param name="v">A <see cref="Vector3D"/> instance.</param>
    //      /// <param name="s">A scalar.</param>
    //      /// <returns>A new <see cref="Vector3D"/> instance containing the sum.</returns>
    //      public static Vec3 operator +(double s, Vec3 v) {
    //          return new Vec3(s + v.v1, s + v.v2, s + v.v3);
    //      }
    //      /// <summary>
    //      /// Subtracts a vector from a vector.
    //      /// </summary>
    //      /// <param name="u">A <see cref="Vector3D"/> instance.</param>
    //      /// <param name="v">A <see cref="Vector3D"/> instance.</param>
    //      /// <returns>A new <see cref="Vector3D"/> instance containing the difference.</returns>
    //      /// <remarks>
    //      ///	result[i] = v[i] - w[i].
    //      /// </remarks>
    //      public static Vec3 operator -(Vec3 u, Vec3 v) {
    //          return new Vec3(u.v1 - v.v1, u.v2 - v.v2, u.v3 - v.v3);
    //      }
    //      /// <summary>
    //      /// Subtracts a scalar from a vector.
    //      /// </summary>
    //      /// <param name="v">A <see cref="Vector3D"/> instance.</param>
    //      /// <param name="s">A scalar.</param>
    //      /// <returns>A new <see cref="Vector3D"/> instance containing the difference.</returns>
    //      /// <remarks>
    //      /// result[i] = v[i] - s
    //      /// </remarks>
    //      public static Vec3 operator -(Vec3 v, double s) {
    //          return new Vec3(v.v1 - s, v.v2 - s, v.v3 - s);
    //      }
    //      /// <summary>
    //      /// Subtracts a vector from a scalar.
    //      /// </summary>
    //      /// <param name="v">A <see cref="Vector3D"/> instance.</param>
    //      /// <param name="s">A scalar.</param>
    //      /// <returns>A new <see cref="Vector3D"/> instance containing the difference.</returns>
    //      /// <remarks>
    //      /// result[i] = s - v[i]
    //      /// </remarks>
    //      public static Vec3 operator -(double s, Vec3 v) {
    //          return new Vec3(s - v.v1, s - v.v2, s - v.v3);
    //      }

    //      /// <summary>
    //      /// Multiplies a vector by a scalar.
    //      /// </summary>
    //      /// <param name="v">A <see cref="Vector3D"/> instance.</param>
    //      /// <param name="s">A scalar.</param>
    //      /// <returns>A new <see cref="Vector3D"/> containing the result.</returns>
    //      public static Vec3 operator *(Vec3 v, double s) {
    //          return new Vec3(s * v.v1, s * v.v2, s * v.v3);
    //      }
    //      /// <summary>
    //      /// Multiplies a vector by a scalar.
    //      /// </summary>
    //      /// <param name="v">A <see cref="Vector3D"/> instance.</param>
    //      /// <param name="s">A scalar.</param>
    //      /// <returns>A new <see cref="Vector3D"/> containing the result.</returns>
    //      public static Vec3 operator *(double s, Vec3 v) {
    //          return new Vec3(s * v.v1, s * v.v2, s * v.v3);
    //      }

    //      public static Vec3 operator *(Vec3 v, Vec3 u) {
    //          return new Vec3(u.v1 * v.v1, u.v2 * v.v2, u.v3 * v.v3);
    //      }

    //      public static Vec3 operator /(Vec3 u, Vec3 v) {
    //          return new Vec3(u.v1 / v.v1, u.v2 / v.v2, u.v3 / v.v3);
    //      }

    //      /// <summary>
    //      /// Divides a vector by a scalar.
    //      /// </summary>
    //      /// <param name="v">A <see cref="Vector3D"/> instance.</param>
    //      /// <param name="s">A scalar</param>
    //      /// <returns>A new <see cref="Vector3D"/> containing the quotient.</returns>
    //      /// <remarks>
    //      /// result[i] = v[i] / s;
    //      /// </remarks>
    //      public static Vec3 operator /(Vec3 u, double s) {
    //          return new Vec3(u.v1 / s, u.v2 / s, u.v3 / s);
    //      }
    //      /// <summary>
    //      /// Divides a scalar by a vector.
    //      /// </summary>
    //      /// <param name="v">A <see cref="Vector3D"/> instance.</param>
    //      /// <param name="s">A scalar</param>
    //      /// <returns>A new <see cref="Vector3D"/> containing the quotient.</returns>
    //      /// <remarks>
    //      /// result[i] = s / v[i]
    //      /// </remarks>
    //      public static Vec3 operator /(double s, Vec3 v) {
    //          return new Vec3(s / v.v1, s / v.v2, s / v.v3);
    //      }
    //      #endregion

    //  }
}
