using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSCode
{
    internal class RSCoder
    {
        public static bool debug = false;

        /// <summary>
        /// RS code over GF(2**mm)
        /// </summary>
        private readonly int mm = 8;

        /// <summary>
        /// nn=2**mm -1   length of codeword
        /// </summary>
        private int nn = (int) Math.Pow(2, 8) - 1;

        /// <summary>
        /// number of errors that can be corrected
        /// </summary>
        private readonly int tt = 4;

        /// <summary>
        /// kk = nn-2*tt
        /// </summary>
        private int kk;

        /// <summary>
        /// generator polynomial
        /// </summary>
        private readonly int[] pp;
        private readonly int[] alpha_to;
        private readonly int[] gg;
        private int[] index_of;

        /// <summary>
        /// received data
        /// </summary>
        private int[] recd;
        private int[] data, bb;

        public RSCoder(int t = 4)
        {
            tt = t;
            kk = nn - 2 * tt;
            pp = new int[] { 1, 0, 1, 1, 1, 0, 0, 0, 1 };

            alpha_to = new int[nn + 1];
            index_of = new int[nn + 1];
            gg = new int[nn - kk + 1];

            recd = new int[nn];
            data = new int[kk];
            bb = new int[nn - kk];

            Generate_gf();
            Gen_poly();
        }

        /// <summary>
        /// generate GF(2**mm) from the irreducible polynomial p(X) in pp[0]..pp[mm]
        /// lookup tables:  index->polynomial form   alpha_to[] contains j=alpha**i;
        ///            polynomial form -> index form  index_of[j=alpha**i] = i
        /// alpha=2 is the primitive element of GF(2**mm)
        /// </summary>
        private void Generate_gf()
        {
            int i, mask;

            mask = 1;
            alpha_to[mm] = 0;
            for (i = 0; i < mm; i++)
            {
                alpha_to[i] = mask;
                index_of[alpha_to[i]] = i;
                if (pp[i] != 0)
                {
                    alpha_to[mm] ^= mask;
                }

                mask <<= 1;
            }
            index_of[alpha_to[mm]] = mm;
            mask >>= 1;
            for (i = mm + 1; i < nn; i++)
            {
                if (alpha_to[i - 1] >= mask)
                {
                    alpha_to[i] = alpha_to[mm] ^ ((alpha_to[i - 1] ^ mask) << 1);
                }
                else
                {
                    alpha_to[i] = alpha_to[i - 1] << 1;
                }

                index_of[alpha_to[i]] = i;
            }
            index_of[0] = -1;
        }

        /// <summary>
        /// Obtain the generator polynomial of the tt-error correcting, length
        /// nn=(2**mm -1) Reed Solomon code  from the product of (X+alpha**i), i=1..2*tt
        /// </summary>
        private void Gen_poly()
        {
            int i, j;

            gg[0] = 2;    // primitive element alpha = 2  for GF(2**mm)
            gg[1] = 1;    // g(x) = (X+alpha) initially
            for (i = 2; i <= nn - kk; i++)
            {
                gg[i] = 1;
                for (j = i - 1; j > 0; j--)
                {
                    if (gg[j] != 0)
                    {
                        gg[j] = gg[j - 1] ^ alpha_to[(index_of[gg[j]] + i) % nn];
                    }
                    else
                    {
                        gg[j] = gg[j - 1];
                    }
                }

                gg[0] = alpha_to[(index_of[gg[0]] + i) % nn];     // gg[0] can never be zero
            }
            // convert gg[] to index form for quicker encoding
            for (i = 0; i <= nn - kk; i++)
            {
                gg[i] = index_of[gg[i]];
            }
        }

        /// <summary>
        /// кодируемые данные передаются через массив data[i], где i=0..(k-1), а сгенерированные символы четности
        /// заносятся в массив b[0]..b[2*t-1]. Исходные и результирующие данные должны быть представлены
        /// в полиномиальной форме (т.е. в обычной форме машинного представления данных).
        /// Кодирование производится с использованием сдвигового feedback-регистра, заполненного соответствующими
        /// элементами массива g[] с порожденным полиномом внутри
        /// Сгенерированное кодовое слово описывается следующей формулой: с(x) = data(x)*x(n-k) + b(x)
        /// </summary>
        private void Encode_rs()
        {
            int i, j;
            int feedback;

            // инициализируем поле бит четности нулями
            for (i = 0; i < nn - kk; i++)
            {
                bb[i] = 0;
            }

            // обрабатываем все символы исходных данных справа налево
            for (i = kk - 1; i >= 0; i--)
            {
                /* готовим (data[i] + b[n – k –1]) к умножению на g[i], т.е. складываем очередной «захваченный»
                 * символ исходных данных с младшим символом битов четности (соответствующего «регистру» b2t-1,
                 * и переводим его в индексную форму, сохраняя результат в регистре feedback,
                 * сумма двух индексов есть произведение полиномов
                 */
                feedback = index_of[data[i] ^ bb[nn - kk - 1]];

                // есть еще символы для обработки?
                if (feedback != -1)
                {

                    // осуществляем сдвиг цепи bx-регистров
                    for (j = nn - kk - 1; j > 0; j--)
                    {
                        /* если текущий коэффициент g – это действительный (т.е. ненулевой коэффициент,
                         * то умножаем feedback на соответствующий g-коэффициент и складываем его
                         * со следующим элементом цепочки
                         */
                        if (gg[j] != -1)
                        {
                            bb[j] = bb[j - 1] ^ alpha_to[(gg[j] + feedback) % nn];
                        }

                        /* если текущий коэффициент g – это нулевой коэффициент, выполняем один лишь
                         * сдвиг без умножения, перемещая символ из одного m-регистра в другой
                         */
                        else
                        {
                            bb[j] = bb[j - 1];
                        }
                    }

                    // закольцовываем выходящий символ в крайний левый b0-регистр
                    bb[0] = alpha_to[(gg[0] + feedback) % nn];
                }

                /* деление завершено, осуществляем последний сдвиг регистра, на выходе регистра
                 * будет частное, которое теряется, а в самом регистре – искомый остаток
                 */
                else
                {
                    for (j = nn - kk - 1; j > 0; j--)
                    {
                        bb[j] = bb[j - 1];
                    }

                    bb[0] = 0;
                }
            }
        }

        /// <summary>
        /// Процедура декодирования кодов Рида-Соломона состоит из нескольких шагов: сначала мы вычисляем
        /// 2t-символьный синдром путем постановки alpha**i в recd(x), где recd – полученное кодовое слово,
        /// предварительно переведенное в индексную форму. По факту вычисления recd(x) мы записываем
        /// очередной символ синдрома в s[i], где i принимает значение от 1 до 2t, оставляя s[0] равным нулю.
        /// Затем, используя итеративный алгоритм Берлекэмпа, мы находим полином локатора ошибки – elp[i].
        /// Если степень elp превышает собой величину t, мы бессильны скорректировать все ошибки и ограничиваемся
        /// выводом сообщения о неустранимой ошибке, после чего совершаем аварийный выход из декодера.
        /// Если же степень elp не превышает t, мы подставляем alpha**i, где i = 1..n в elp для вычисления
        /// корней полинома. Обращение найденных корней дает нам позиции искаженных символов. Если количество
        /// определенных позиций искаженных символов меньше степени elp, искажению подверглось более чем t
        /// символов и мы не можем восстановить их. Во всех остальных случаях восстановление оригинального
        /// содержимого искаженных символов вполне возможно. В случае, когда количество ошибок заведомо велико,
        /// для их исправления декодируемые символы проходят сквозь декодер без каких-либо изменений.
        /// </summary>
        public void Decode_rs()
        {
            int i, j, u, q;
            int[] s = new int[nn - kk + 1];             // полином синдрома ошибки
            int[,] elp = new int[nn - kk + 2, nn - kk]; // полином локатора ошибки лямбда
            int[] d = new int[nn - kk + 2];
            int[] l = new int[nn - kk + 2];
            int[] u_lu = new int[nn - kk + 2];
            int count = 0, syn_error = 0;
            int[] root = new int[tt];
            int[] loc = new int[tt];
            int[] z = new int[tt + 1];
            int[] err = new int[nn];
            int[] reg = new int[tt + 1];

            // вычисляем синдром
            for (i = 1; i <= nn - kk; i++)
            {
                s[i] = 0;   // инициализация s-регистра (на его вход по умолчанию поступает ноль)

                /* выполняем s[i] += recd[j]*ij т.е. берем очередной символ декодируемых данных, умножаем его
                 * на порядковый номер данного символа, умноженный на номер очередного оборота и складываем
                 * полученный результат с содержимым s-регистра по факту исчерпания всех декодируемых символов,
                 * мы повторяем весь цикл вычислений опять – по одному разу для каждого символа четности
                 */
                for (j = 0; j < nn; j++)
                {
                    if (recd[j] != -1)
                    {
                        s[i] ^= alpha_to[(recd[j] + i * j) % nn];
                    }
                }
                if (s[i] != 0)
                {
                    syn_error = 1;        // если синдром не равен нулю, взводим флаг ошибки
                }

                // преобразуем синдром из полиномиальной формы в индексную
                s[i] = index_of[s[i]];
            }

            // коррекция ошибок
            if (syn_error != 0) // если есть ошибки, пытаемся их скорректировать
            {
                if (debug)
                {
                    Console.WriteLine("There are errors");
                }
                // вычисление полинома локатора лямбда
                /* вычисляем полином локатора ошибки через итеративный алгоритм Берлекэмпа. Следуя терминологии
                 * Lin and Costello (см. "Error Control Coding: Fundamentals and Applications" Prentice Hall 1983
                 * ISBN 013283796) d[u] представляет собой m («мю»), выражающую расхождение (discrepancy),
                 * где u = m + 1 и m есть номер шага из диапазона от –1 до 2t. У Блейхута та же самая величина
                 * обозначается D(x) («дельта») и называется невязкой. l[u] представляет собой степень elp
                 * для данного шага итерации, u_l[u] представляет собой разницу между номером шага и степенью elp,
                 */

                // инициализируем элементы таблицы
                d[0] = 0;           // индексная форма
                d[1] = s[1];        // индексная форма
                elp[0, 0] = 0;      // индексная форма
                elp[1, 0] = 1;      // полиномиальная форма
                for (i = 1; i < nn - kk; i++)
                {
                    elp[0, i] = -1;   // индексная форма
                    elp[1, i] = 0;    // полиномиальная форма
                }
                l[0] = 0;
                l[1] = 0;
                u_lu[0] = -1;
                u_lu[1] = 0;
                u = 0;

                do
                {
                    u++;
                    if (d[u] == -1)
                    {
                        l[u + 1] = l[u];
                        for (i = 0; i <= l[u]; i++)
                        {
                            elp[u + 1, i] = elp[u, i];
                            elp[u, i] = index_of[elp[u, i]];
                        }
                    }
                    else
                    {// поиск слов с наибольшим u_lu[q], таких что d[q]!=0
                        q = u - 1;
                        while ((d[q] == -1) && (q > 0))
                        {
                            q--;
                        }

                        // найден первый ненулевой d[q]
                        if (q > 0)
                        {
                            j = q;
                            do
                            {
                                j--;
                                if ((d[j] != -1) && (u_lu[q] < u_lu[j]))
                                {
                                    q = j;
                                }
                            } while (j > 0);
                        }

                        /* как только мы найдем q, такой что d[u]!=0 и u_lu[q] есть максимум
                         * запишем степень нового elp полинома
                         */
                        if (l[u] > l[q] + u - q)
                        {
                            l[u + 1] = l[u];
                        }
                        else
                        {
                            l[u + 1] = l[q] + u - q;
                        }

                        // формируем новый elp(x)
                        for (i = 0; i < nn - kk; i++)
                        {
                            elp[u + 1, i] = 0;
                        }

                        for (i = 0; i <= l[q]; i++)
                        {
                            if (elp[q, i] != -1)
                            {
                                elp[u + 1, i + u - q] = alpha_to[(d[u] + nn - d[q] + elp[q, i]) % nn];
                            }
                        }

                        for (i = 0; i <= l[u]; i++)
                        {
                            elp[u + 1, i] ^= elp[u, i];
                            elp[u, i] = index_of[elp[u, i]];  // преобразуем старый elp в индексную форму
                        }
                    }
                    u_lu[u + 1] = u - l[u + 1];

                    // формируем (u + 1) невязку

                    if (u < nn - kk)    // на последней итерации расхождение не было обнаружено
                    {
                        if (s[u + 1] != -1)
                        {
                            d[u + 1] = alpha_to[s[u + 1]];
                        }
                        else
                        {
                            d[u + 1] = 0;
                        }

                        for (i = 1; i <= l[u + 1]; i++)
                        {
                            if ((s[u + 1 - i] != -1) && (elp[u + 1, i] != 0))
                            {
                                d[u + 1] ^= alpha_to[(s[u + 1 - i] + index_of[elp[u + 1, i]]) % nn];
                            }
                        }

                        d[u + 1] = index_of[d[u + 1]];    // переводим d[u+1] в индексную форму
                    }
                } while ((u < nn - kk) && (l[u + 1] <= tt));

                // расчет локатора завершен

                u++;
                if (l[u] <= tt)
                {   // коррекция ошибок возможна

                    // переводим elp в индексную форму
                    for (i = 0; i <= l[u]; i++)
                    {
                        elp[u, i] = index_of[elp[u, i]];
                    }

                    // нахождение корней полинома локатора ошибки

                    for (i = 1; i <= l[u]; i++)
                    {
                        reg[i] = elp[u, i];
                    }

                    count = 0;
                    for (i = 1; i <= nn; i++)
                    {
                        q = 1;
                        for (j = 1; j <= l[u]; j++)
                        {
                            if (reg[j] != -1)
                            {
                                reg[j] = (reg[j] + j) % nn;
                                q ^= alpha_to[reg[j]];
                            }
                        }

                        if (0 == q)
                        {   // записываем корень и индекс позиции ошибки
                            root[count] = i;
                            loc[count] = nn - i;
                            count++;
                        }
                    }
                    if (count == l[u])
                    {   // нет корней – степень elp < t ошибок

                        // формируем полином z(x)
                        for (i = 1; i <= l[u]; i++)
                        {   // Z[0] всегда равно 1
                            if ((s[i] != -1) && (elp[u, i] != -1))
                            {
                                z[i] = alpha_to[s[i]] ^ alpha_to[elp[u, i]];
                            }
                            else if ((s[i] != -1) && (elp[u, i] == -1))
                            {
                                z[i] = alpha_to[s[i]];
                            }
                            else if ((s[i] == -1) && (elp[u, i] != -1))
                            {
                                z[i] = alpha_to[elp[u, i]];
                            }
                            else
                            {
                                z[i] = 0;
                            }

                            for (j = 1; j < i; j++)
                            {
                                if ((s[j] != -1) && (elp[u, i - j] != -1))
                                {
                                    z[i] ^= alpha_to[(elp[u, i - j] + s[j]) % nn];
                                }
                            }

                            z[i] = index_of[z[i]];  // переводим z[i] в индексную форму
                        }

                        // вычисление значения ошибок в позициях loc[i]

                        for (i = 0; i < nn; i++)
                        {
                            err[i] = 0;
                            if (recd[i] != -1)  // переводим recd[] в полиномиальную форму
                            {
                                recd[i] = alpha_to[recd[i]];
                            }
                            else
                            {
                                recd[i] = 0;
                            }
                        }
                        for (i = 0; i < l[u]; i++)
                        {   // сначала вычисляем числитель ошибки
                            err[loc[i]] = 1;       // accounts for z[0]
                            for (j = 1; j <= l[u]; j++)
                            {
                                if (z[j] != -1)
                                {
                                    err[loc[i]] ^= alpha_to[(z[j] + j * root[i]) % nn];
                                }
                            }

                            if (err[loc[i]] != 0)
                            {
                                err[loc[i]] = index_of[err[loc[i]]];
                                q = 0;      // формируем знаменателькоэффициента ошибки
                                for (j = 0; j < l[u]; j++)
                                {
                                    if (j != i)
                                    {
                                        q += index_of[1 ^ alpha_to[(loc[j] + root[i]) % nn]];
                                    }
                                }

                                q = q % nn;
                                err[loc[i]] = alpha_to[(err[loc[i]] - q + nn) % nn];
                                recd[loc[i]] ^= err[loc[i]];  // recd[i] должен быть в полиномиальной форме
                            }
                        }
                    }
                    else
                    {   // нет корней, решение системы уравнений невозможно, т.к. степень elp >= t

                        for (i = 0; i < nn; i++)
                        {        // could return error flag if desired
                            if (recd[i] != -1)
                            {        // переводим recd[] в полиномиальную форму
                                recd[i] = alpha_to[recd[i]];
                            }
                            else
                            {
                                recd[i] = 0;     // выводим информационное слово как есть
                            }
                        }
                        if (debug)
                        {
                            Console.WriteLine("Can not solve 1");
                        }
                    }
                }
                else
                {   // степень elp > t, решение невозможно

                    for (i = 0; i < nn; i++)
                    {       // could return error flag if desired
                        if (recd[i] != -1)
                        {       // переводим recd[] в полиномиальную форму
                            recd[i] = alpha_to[recd[i]];
                        }
                        else
                        {
                            recd[i] = 0;     // выводим информационное слово как есть
                        }
                    }
                    if (debug)
                    {
                        Console.WriteLine("Can not solve 2");
                    }
                }
            }
            else
            {   // ошибок не обнаружено
                for (i = 0; i < nn; i++)
                {
                    if (recd[i] != -1)
                    {        // convert recd[] to polynomial form
                        recd[i] = alpha_to[recd[i]];
                    }
                    else
                    {
                        recd[i] = 0;
                    }
                }
            }
        }

        public static byte[] Encode(byte[] data, int t = 4)
        {
            if (t < 1 || t > 5)
            {
                throw new ArgumentOutOfRangeException();
            }

            RSCoder coder = new RSCoder(t);

            byte[] dataWithLen = new byte[data.Length + sizeof(int)];
            int size = data.Length;
            for (int i = 0; i < sizeof(int); ++i)
            {
                dataWithLen[i] = (byte) (size % 256);
                size /= 256;
            }
            for (int i = 0; i < data.Length; ++i)
            {
                dataWithLen[i + sizeof(int)] = data[i];
            }

            byte[] input = dataWithLen;

            int blocksCnt = (int) Math.Ceiling(input.Length / (double) coder.kk);
            byte[] output = new byte[blocksCnt * coder.nn];
            for (int i = 0; i < blocksCnt; ++i)
            {
                if (debug)
                {
                    Console.WriteLine("INPUT[{0:D}]", i);
                }

                for (int j = 0; j < coder.kk; ++j)
                {
                    coder.data[j] = (input.Length > j + i * coder.kk) ? input[j + i * coder.kk] : 0;
                    if (debug)
                    {
                        Console.Write("{0:D} ", coder.data[j]);
                    }
                }
                if (debug)
                {
                    Console.WriteLine();
                }

                coder.Encode_rs();

                if (debug)
                {
                    Console.WriteLine("OUTPUT[{0:D}]", i);
                }

                for (int j = 0; j < coder.nn - coder.kk; ++j)
                {
                    output[j + i * coder.nn] = (byte) coder.bb[j];
                    if (debug)
                    {
                        Console.Write("{0:D} ", output[j + i * coder.nn]);
                    }
                }

                for (int j = 0; j < coder.kk; ++j)
                {
                    output[j + i * coder.nn + coder.nn - coder.kk] = (byte) coder.data[j];
                    if (debug)
                    {
                        Console.Write("{0:D} ", output[j + i * coder.nn + coder.nn - coder.kk]);
                    }
                }
                if (debug)
                {
                    Console.WriteLine();
                }
            }
            return output;
        }

        public static byte[] Decode(byte[] data, int t = 4)
        {
            if (t < 1 || t > 5)
            {
                throw new ArgumentOutOfRangeException();
            }

            RSCoder coder = new RSCoder(t);

            byte[] input = data;

            int blocksCnt = (int) Math.Ceiling(input.Length / (double) coder.nn);
            byte[] output = new byte[blocksCnt * coder.kk];
            for (int i = 0; i < blocksCnt; ++i)
            {
                if (debug)
                {
                    Console.WriteLine("INPUT[{0:D}]", i);
                }

                for (int j = 0; j < coder.nn; ++j)
                {
                    coder.recd[j] = (input.Length > j + i * coder.nn) ? coder.index_of[input[j + i * coder.nn]] : 0;
                    if (debug)
                    {
                        Console.Write("{0:D} ", coder.recd[j]);
                    }
                }
                if (debug)
                {
                    Console.WriteLine();
                }

                coder.Decode_rs();

                if (debug)
                {
                    Console.WriteLine("OUTPUT[{0:D}]", i);
                }

                for (int j = 0; j < coder.kk; ++j)
                {
                    output[j + i * coder.kk] = (byte) (coder.recd[j + coder.nn - coder.kk]);
                    if (debug)
                    {
                        Console.Write("{0:D} ", output[j + i * coder.kk]);
                    }
                }
                if (debug)
                {
                    Console.WriteLine();
                }
            }

            int size = 0;
            for (int i = sizeof(int) - 1; i >= 0; --i)
            {
                size = size * 256 + output[i];
            }

            if ((size <= 0) || (size > output.Length + sizeof(int)))
            {
                throw new Exception(String.Format("Incorrect size = {0:D}!", size));
            }

            byte[] decoded = new byte[size];

            for (int i = 0; i < decoded.Length; ++i)
            {
                decoded[i] = output[i + sizeof(int)];
            }

            return decoded;
            //return output;
        }

        private static void WriteBits(BitArray bits, int sep = 8)
        {
            int i = 0;
            foreach (bool b in bits)
            {
                Console.Write(b ? 1 : 0);
                if (++i % sep == 0)
                {
                    Console.Write(" ");
                }
                if (i % 40 == 0)
                {
                    Console.WriteLine();
                }
            }
            Console.WriteLine();
        }

        public static void Test()
        {
            Random rnd = new Random();
            for (int i = 0; i < 100; ++i)
            {
                byte[] data = new byte[rnd.Next(1, 4097)];
                for (int j = 0; j < data.Length; ++j)
                {
                    data[j] = (byte) (rnd.Next(256));
                }

                byte[] decoded = null;
                try
                {
                    decoded = Decode(Encode(data, 4), 4);
                }
                catch (Exception)
                {
                    Console.Write("data = {");
                    for (int j = 0; j < data.Length; ++j)
                    {
                        Console.Write("{0:D}, ", data[j]);
                    }
                    Console.WriteLine("};");
                    break;
                }

                Console.WriteLine("I: {0:D}, Length: {1:D}", i, data.Length);

                if (decoded.Length != data.Length)
                {
                    Console.WriteLine("Размеры не совпали! data.Len = {0:D} decoded.Len = {1:D}", data.Length, decoded.Length);
                    break;
                }
                else
                {
                    bool error = false;
                    for (int j = 0; j < data.Length; ++j)
                    {
                        if (data[j] != decoded[j])
                        {
                            Console.WriteLine("Несовпадение в позиции {0:D} ({1:D} != {2:D})", j, data[j], decoded[j]);
                            error = true;
                        }
                    }
                    if (error)
                    {
                        Console.Write("data = {");
                        for (int j = 0; j < data.Length; ++j)
                        {
                            Console.Write("{0:D}, ", data[j]);
                        }
                        Console.WriteLine("};");
                        break;
                    }
                }
                rnd.Next();
            }
        }

        public static void _Main()
        {
            //Test();
            /*
            byte[] data = { 64, 21 };
            byte[] encoded = encode(data);
            byte[] decoded = decode(encoded);

            Console.Write("data: ");
            for (int j = 0; j < data.Length; ++j) {
                Console.Write("{0:D}, ", data[j]);
            }
            Console.WriteLine();

            Console.Write("encoded: ");
            for (int j = 0; j < encoded.Length; ++j) {
                Console.Write("{0:D}, ", encoded[j]);
            }
            Console.WriteLine();
            
            Console.Write("decoded: ");
            for (int j = 0; j < decoded.Length; ++j) {
                Console.Write("{0:D}, ", decoded[j]);
            }
            Console.WriteLine();
            */
            byte[] data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            foreach (var el in data)
            {
                for (byte m = 0b10000000; m > 0; m >>= 1)
                {
                    Console.Write((el & m) == 0 ? 0 : 1);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            byte[] encoded = RSCoder.Encode(data, 4);
            int i = 0;
            foreach (var el in encoded)
            {
                for (byte m = 0b10000000; m > 0; m >>= 1)
                {
                    Console.Write((el & m) == 0 ? 0 : 1);
                }
                if (i++ % (encoded.Length / data.Length) == 0)
                {
                    Console.WriteLine();
                }
                else
                {
                    Console.Write(' ');
                }
            }
        }
    }
}
