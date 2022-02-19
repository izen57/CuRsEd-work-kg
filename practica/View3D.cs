using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace practica
{
	class View3D
	{
		ZBuffer ZB0, ZB;
		readonly double zNear;
		readonly double zFar;
		Matrix4D P, M, T; // матрицы преобразования (объектив), модельная матрица (располжение наблюдателя), матриц

		public double C0 = 15;
		Random R = new Random();

		Color cSky = Color.White, bC = Color.White;
		public double view_x0 = 0, view_y0 = 0;
		int n_ships;
		Vector4D[] ships = new Vector4D[100];

		int n_balls;
		Vector4D[] balls = new Vector4D[100];

		int n_clouds;
		Vector4D[] clouds = new Vector4D[100];
		double[] clouds_r = new double[100];

		public View3D()
		{
			zNear = 0.1;
			zFar = 800;

			n_ships = 2;
			ships[0] = new Vector4D(50, 0, 150); // -75, 7 240
			//ships[1] = new Vector4D(-100, -80, 80); // -100, -80, 80
			ships[1] = new Vector4D(500, -100, 200); //120, -100, 7

			n_balls = 0;
			balls[0] = new Vector4D(280, 240, 300);
			balls[1] = new Vector4D(-260, -280, 300);
		}

		public void AddShip()
		{
			if (R.Next() % 3 == 1)
			{
				var zz = 360 + 240 * R.NextDouble();
				var x0 = -4 * zz + 8 * zz * R.NextDouble();
				ships[n_ships] = new Vector4D(x0, 7 - 0.00001 * Math.Pow(x0 - 0.5 * ZB.width, 2), zz);
			}
			else
				ships[n_ships] = new Vector4D(-400 + 800 * R.NextDouble(), -70 - 180 * R.NextDouble(), 70 + 70 * R.NextDouble());

			 n_ships++;
		}

		public void AddBall()
		{
			balls[n_balls] = new Vector4D(-1500 + 3000 * R.NextDouble(), 220 + 180 * R.NextDouble(), 300 + 400 * R.NextDouble());
			n_balls++;
		}

		public void AddCloud()
		{
			clouds[n_clouds] = new Vector4D(-800 + 1600 * R.NextDouble(), 0, 750);
			clouds_r[n_clouds] = 36 + 144 * R.NextDouble();

			n_clouds++;
		}

		public void ClearScene()
		{
			n_ships = n_balls = n_clouds = 0;
		}

		public UInt64[] ColorBuffer()
		{
			return ZB.GetColorBuffer();
		}

		public void SetViewPort(int w, int h)
		{
			ZB0 = new ZBuffer(w, h, zNear, zFar);
			ZB = new ZBuffer(w, h, zNear, zFar);
		}

		void Clear(Color c, Color cSky)
		{
			bC = c;
			this.cSky = cSky;
			ZB0.Clear(Graphicx.ColorToInt(c), Graphicx.ColorToInt(cSky));
			ZB.Clear(Graphicx.ColorToInt(c), Graphicx.ColorToInt(cSky));
		}

		Matrix4D Perspective(double angle0, double aspectRatio, double zNear, double zFar)
		{
			var angle = angle0 * Math.PI / 180;
			var A = Matrix4D.Diagonal
			(
				1 / (aspectRatio * Math.Tan(angle / 2)),
				1 / Math.Tan(angle / 2),
				-(zFar + zNear) / (zFar - zNear),
				0
			);

			A[2, 3] = -1;
			A[3, 2] = -(/*2 **/zFar * zNear) / (zFar - zNear);
			return A;
		}

		Matrix4D Orto(double scale, double aspectRatio)
		{
			return Matrix4D.Diagonal(scale, aspectRatio * scale, 1, 1);
		}

		void SetUpCamera()
		{
			T = Matrix4D.Identity();
			P = Orto(100, 1.0 * ZB.height / ZB.width);
			T = T * P;
			M = Matrix4D.LookAt(new Vector4D(8, 8, 3), Vector4D.Zero(), new Vector4D(0, 0, 1));
			T = T * M;
		}

		Vector4D GetScreenCoordinates(Vector4D a)
		{
			double K = 180;
			double L = a.Z + K;
			double al = Math.Atan(view_x0 / 300);
			double dx = Math.Cos(al) * a.X - Math.Sin(al) * (a.Z - 240);
			double dz = Math.Sin(al) * a.X + Math.Cos(al) * (a.Z - 240);

			double x = 1.5 * K * dx / L, y = 1.5 * K * a.Y / L;

			return new Vector4D(ZB.width / 2 + x, ZB.height / 2 + y, 240 + dz);
		}


		void DrawShips()
		{
			// север справа
			for (int i = 0; i < n_ships; i++)
			{
				var u = ships[i] - new Vector4D(0, 20 * (15 - C0) / 15 - (1 - Math.Pow(C0 / 15, 0.33)) * 5, 0);

				var color = Color.DarkOrange;
				var color2 = Color.DarkGreen;

				// днище корабля
				Quad
				(
					color,
					T0(u, Vector4D.Point(-60, 5, -16)),
					T0(u, Vector4D.Point(60, 5, -16)),
					T0(u, Vector4D.Point(60, 5, 16)),
					T0(u, Vector4D.Point(-60, 5, 16))
				);
				// передняя к камере (восточная) часть корпуса
				Quad
				(
					color,
					T0(u, Vector4D.Point(-60, 5, -16)), //-30, 5, -8
					T0(u, Vector4D.Point(60, 5, -16)), //30, 5, -8
					T0(u, Vector4D.Point(66, 50, -24)), // 33, 15, -12
					T0(u, Vector4D.Point(-66, 50, -24)) // -33, 15, -12
				);
				// задняя (западная) часть корпуса
				Quad
				(
					color,
					T0(u, Vector4D.Point(-60, 5, 16)),
					T0(u, Vector4D.Point(60, 5, 16)),
					T0(u, Vector4D.Point(66, 50, 24)),
					T0(u, Vector4D.Point(-66, 50, 24))
				);
				// южная часть корпуса
				Quad
				(
					color,
					T0(u, Vector4D.Point(-60, 5, -16)), // -3, 5, -8
					T0(u, Vector4D.Point(-78, 50, -16)), // -39, 25, -8
					T0(u, Vector4D.Point(-78, 50, 16)), // -39, 25, 8
					T0(u, Vector4D.Point(-60, 5, 16)) // -30, 5, 8
				);
				// северная часть корпуса
				Quad
				(
					color,
					T0(u, Vector4D.Point(60, 5, -16)),
					T0(u, Vector4D.Point(96, 50, -4)),
					T0(u, Vector4D.Point(96, 50, 4)),
					T0(u, Vector4D.Point(60, 5, 16))
				);
				// юго-восточная часть
				Triangle
				(
					color,
					T0(u, Vector4D.Point(-60, 5, -16)), // -30, 5, -8
					T0(u, Vector4D.Point(-66, 50, -24)), // -33, 15, -12
					T0(u, Vector4D.Point(-78, 50, -16)) // -39, 15, -8
				);
				// северо-восточная часть
				Triangle
				(
					color,
					T0(u, Vector4D.Point(60, 5, -16)), // 30, 5, -8
					T0(u, Vector4D.Point(66, 50, -24)), // 33, 15, -12
					T0(u, Vector4D.Point(96, 50, -4)) // 48, 15, -2
				);
				// юго-западная часть
				Triangle
				(
					color,
					T0(u, Vector4D.Point(-60, 5, 16)), // -30, 5, 8
					T0(u, Vector4D.Point(-66, 50, 24)), // -33, 15, 12
					T0(u, Vector4D.Point(-78, 50, 16)) // -39, 15, 8
				);
				// северо-западная часть
				Triangle
				(
					color,
					T0(u, Vector4D.Point(60, 5, 16)), // 30, 5, 8
					T0(u, Vector4D.Point(66, 50, 24)), // 33, 15, 12
					T0(u, Vector4D.Point(96, 50, 4)) // 48, 15, 2
				);
				// прямоугольная часть палубы
				Quad
				(
					color,
					T0(u, Vector4D.Point(-66, 50, -24)), // -33, 15, -12
					T0(u, Vector4D.Point(66, 50, -24)), // 33, 15, -12
					T0(u, Vector4D.Point(66, 50, 24)), //33, 15, 12
					T0(u, Vector4D.Point(-66, 50, 24)) //-33, 15, 12
				);
				// задняя часть палубы
				Quad
				(
					color,
					T0(u, Vector4D.Point(-66, 50, -24)), // -33, 15, -12
					T0(u, Vector4D.Point(-78, 50, -16)), // -39, 15, -8
					T0(u, Vector4D.Point(-78, 50, 16)), // -39, 15, 8
					T0(u, Vector4D.Point(-66, 50, 24)) // -33, 15, 12
				);
				// передняя часть палубы
				Quad
				(
					color,
					T0(u, Vector4D.Point(96, 50, -4)),
					T0(u, Vector4D.Point(66, 50, -24)),
					T0(u, Vector4D.Point(66, 50, 24)),
					T0(u, Vector4D.Point(96, 50, 4))
				);
				// восточная часть трубы
				Quad
				(
					color2,
					T0(u, Vector4D.Point(-40, 50, -10)), // -20, 15, -5
					T0(u, Vector4D.Point(16, 50, -10)), // 8, 15, -5
					T0(u, Vector4D.Point(4, 76, -9)), // 4, 28, -3
					T0(u, Vector4D.Point(-36, 76, -9)) // -18, 28, -3
				);
				// западная часть трубы
				Quad
				(
					color2,
					T0(u, Vector4D.Point(-40, 50, 10)), // -20, 15, 5
					T0(u, Vector4D.Point(16, 50, 10)), // 8, 15, 5
					T0(u, Vector4D.Point(8, 76, 9)), // 4, 28, 3
					T0(u, Vector4D.Point(-36, 76, 9)) // -18, 28, 3
				);
				// южная часть трубы
				Quad
				(
					color2,
					T0(u, Vector4D.Point(-40, 50, 10)), // -20, 15, 5
					T0(u, Vector4D.Point(-36, 76, 9)), // -18, 28, 3
					T0(u, Vector4D.Point(-36, 76, -9)), // -18, 28, -3
					T0(u, Vector4D.Point(-40, 50, -10)) // -20, 15, -5
				);
				// северная часть трубы
				Quad
				(
					color2,
					T0(u, Vector4D.Point(16, 50, 10)), // 8, 15, 5
					T0(u, Vector4D.Point(8, 76, 9)), // 4, 28, 3
					T0(u, Vector4D.Point(8, 76, -9)), // 4, 28, -3
					T0(u, Vector4D.Point(16, 50, -10)) // 8, 15, -5
				);
				// верхняя часть трубы
				Quad
				(
					color2,
					T0(u, Vector4D.Point(-36, 76, 9)), // -18, 28, 3
					T0(u, Vector4D.Point(8, 76, 9)), // 4, 28, 3
					T0(u, Vector4D.Point(8, 76, -9)), // 4, 28, -3
					T0(u, Vector4D.Point(-36, 76, -9)) // -18, 28, -3
				);
			}
		}

		void DrawBalls()
		{
			for (int k = 0; k < n_balls; k++)
			{
				var center = balls[k];
				//
				double r = 36 * 2;
				//var center = new Vector4D(98, 80, 130);
				double db = Math.PI / 2 / 12;
				var color3 = Color.Magenta;
				for (int i = -12; i < 12; i++)
				{
					int n1 = 24;
					double da = 2 * Math.PI / n1;
					for (int j = 0; j < n1; j++)
					{
						var p1 = new Vector4D
						(
							r * Math.Cos(i * db) * Math.Cos(j * da),
							r * Math.Sin(i * db),
							r * Math.Cos(i * db) * Math.Sin(j * da)
						);
						var p2 = new Vector4D
						(
							r * Math.Cos(i * db) * Math.Cos((j + 1) * da),
							r * Math.Sin(i * db),
							r * Math.Cos(i * db) * Math.Sin((j + 1) * da)
						);
						var p3 = new Vector4D
						(
							r * Math.Cos((i + 1) * db) * Math.Cos((j + 1) * da),
							r * Math.Sin((i + 1) * db),
							r * Math.Cos((i + 1) * db) * Math.Sin((j + 1) * da)
						);
						var p4 = new Vector4D
						(
							r * Math.Cos((i + 1) * db) * Math.Cos(j * da),
							r * Math.Sin((i + 1) * db),
							r * Math.Cos((i + 1) * db) * Math.Sin(j * da)
						);
						Quad(color3, center + p1, center + p2, center + p3, center + p4);
					}
				}
			}
		}

		void DrawClouds()
		{
			for (int k = 0; k < n_clouds; k++)
			{
				var center = clouds[k];
				double r = clouds_r[k];
				var color3 = Color.DeepSkyBlue;
				{
					int n1 = 24;
					double da = 2 * Math.PI / n1;
					for (int j = 0; j < n1; j++)
					{
						var p1 = new Vector4D(0, 0, 0);
						var p2 = new Vector4D(r * Math.Cos(j * da), r * Math.Sin(j * da), 0);
						var p3 = new Vector4D(r * Math.Cos((j + 1) * da), r * Math.Sin((j + 1) * da), 0);

						Triangle(color3, center + p1, center + p2, center + p3);
					}
				}
			}
		}

		void Draw()
		{
			DrawClouds();
			ZB0.Clear(Graphicx.ColorToInt(bC), Graphicx.ColorToInt(cSky), false);
			DrawShips();
			DrawBalls();
		}

		Vector4D T0(Vector4D u, Vector4D p)
		{
			return new Vector4D(u.X + 1.33 * p.X, u.Y + 1.33 * p.Y, u.Z + p.Z);
		}

		Vector4D T1(Vector4D p)
		{
			return new Vector4D(-100 + 1.33 * p.X, -80 + 1.33 * p.Y, 80 + p.Z);
		}

		public void Display()
		{
			Clear(Color.DodgerBlue, Color.AliceBlue);
			SetUpCamera();
			Draw();
			ApplyEffect();
		}

		#region Primitives
		Color MixColor(Color c1, Color c2, double tau)
		{
			int r = (int) ((1 - tau) * c1.R + tau * c2.R);
			int g = (int) ((1 - tau) * c1.G + tau * c2.G);
			int b = (int) ((1 - tau) * c1.B + tau * c2.B);
			return Color.FromArgb(r, g, b);
		}

		Color MixColor(Color c, double t)
		{
			double r = c.R, g = c.G, b = c.B;
			if (t < 0)
				t = -t;
			if (t > 1)
				t = 1;
			if (t < 0.5)
			{
				double tau = 2 * t;
				r = tau * r;
				g = tau * g;
				b = tau * b;
			}
			else
			{
				double tau = 2 * t - 1;
				r = (1 - tau) * r + tau * 255;
				g = (1 - tau) * g + tau * 255;
				b = (1 - tau) * b + tau * 255;
			}
			return Color.FromArgb((int) r, (int) g, (int) b);
		}

		void Triangle(Color c, Vector4D p0, Vector4D p1, Vector4D p2)
		{
			var q0 = /*T **/GetScreenCoordinates(p0);
			var q1 = GetScreenCoordinates(p1);
			var q2 = GetScreenCoordinates(p2);
			var u = new Vector4D(q1.X - q0.X, q1.Y - q0.Y, q1.Z - q0.Z);
			var v = new Vector4D(q2.X - q0.X, q2.Y - q0.Y, q2.Z - q0.Z);

			var normal = new Vector4D(u.Y * v.Z - u.Z * v.Y, u.Z * v.X - u.X * v.Z, u.X * v.Y - u.Y * v.X);
			if (normal.Lenght3D() > 1e-7)
				normal = normal.Normalized3D();
			var angle = Math.Atan(view_x0 / 240);
			var light_v = new Vector4D(Math.Cos(Math.PI / 4 + 0 * angle), -1, Math.Cos(Math.PI / 4 + 0 * angle)).Normalized3D();
			double t = light_v * normal;
			c = MixColor(c, t);

			ZB0.Triangle(Graphicx.ColorToInt(c), (int) q0.X, (int) q0.Y, (int) q0.Z, (int) q1.X, (int) q1.Y, (int) q1.Z, (int) q2.X, (int) q2.Y, (int) q2.Z);
		}

		void Quad(Color c, Vector4D p0, Vector4D p1, Vector4D p2, Vector4D p3)
		{
			Triangle(c, p0, p1, p2);
			Triangle(c, p0, p2, p3);
		}

		void Line(Color c, Vector4D p0, Vector4D p1)
		{
			var q0 = /*T **/GetScreenCoordinates(p0);
			var q1 = GetScreenCoordinates(p1);

			ZB.Line(Graphicx.ColorToInt(c), (int) q0.X, (int) q0.Y, (int) q0.Z, (int) q1.X, (int) q1.Y, (int) q1.Z);
		}
		#endregion

		void ApplyEffect()
		{
			using (StreamWriter logfile = new StreamWriter(@"log.txt", true))
			{
				Stopwatch stopWatch = new Stopwatch();
				stopWatch.Start();

				var seaColorI = Graphicx.ColorToInt(Color.DodgerBlue); // настройка цвета моря
				var skyColorI = Graphicx.ColorToInt(Color.AliceBlue); // настройка цвета неба
				var DeepSkyBlueI = Graphicx.ColorToInt(Color.DeepSkyBlue); // настройка цвета далёкого неба
				UInt64[] B0 = ZB0.GetColorBuffer(); // получение z-буфера цвета и запись в массив
				UInt64[] B = ZB.GetColorBuffer();
				double ds = C0; // коэффициент преломления луча
				for (int i = 0; i < ZB.height; i++) // проход по z-буферу
					for (int j = 0; j < ZB.width; j++)
					{
						double y = i - 0.5 * ZB.height; // получение аппликата пикселя
						if (i < 0.5 * ZB.height - ds) // если разница половины высоты буфера и коэф. прел. больше, чем i-ая позиция пикселя
						{
							B[i * ZB0.width + j] = B0[i * ZB0.width + j]; // то переносим значение цвета в буфер
						}
						else
						{
							// расчёт искажения при изменении коэффициента угла преломления
							double y2 = ds < 1 ? y : y + 2.3 * ds / (1 + Math.Pow(y + ds, 1.5) / ds);
							//y2 = -y;
							var i2 = (int) (0.5 * ZB.height + y2);

							// перевод цвета в цветовой код
							var c0 = Graphicx.IntToColor(B0[i * ZB0.width + j]);
							var c1 = Graphicx.IntToColor(B0[i2 * ZB0.width + j]);

							// если полученные цвета не соотвествуют изначальным, то
							if (B0[i2 * ZB0.width + j] != skyColorI && B0[i2 * ZB0.width + j] != DeepSkyBlueI)
								B[i * ZB0.width + j] = Graphicx.ColorToInt(MixColor(c0, c1, 0.5)); // смешиваем цвета и переносим значение цвета в буфер
							else
								B[i * ZB0.width + j] = B0[i * ZB0.width + j]; // иначе переносим значение цвета в буфер
						}
					}

				stopWatch.Stop();
				long nanosecPerTick = 1000L * 1000L * 1000L / Stopwatch.Frequency;
				logfile.WriteLine($"Время применения эффекта с коэффициентом {ds} в наносекундах: " + stopWatch.ElapsedTicks * nanosecPerTick + '\n');
			}
		}
	}
}
