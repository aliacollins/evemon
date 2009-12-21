using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace UnitTester
{
	class Program
	{
		static IEnumerable<string> _words = "the quick brown lynx".Split ();

		static void Assert (bool condition)
		{
			Debug.Assert (condition);
		}

		static void Main (string [] args)
		{
			TestAverage ();
			TestSum ();
			TestCount ();
			TestMin ();
			TestMax ();
			TestConverters ();
			TestElement ();
			TestFiltering ();
			TestGeneration ();
			TestGrouping ();
			TestJoining ();
			TestMisc ();
			TestOrdering ();
			TestProjection ();
			TestQuantifiers ();
			TestSet ();

			Console.WriteLine ("All tests passed");
			Console.ReadLine ();
		}

		static void TestAverage ()
		{
			Assert (_words.Average (w => w.Length) - 4.25 < .00001);
			Assert (_words.Select (w => w.Length).Average () - 4.25 < .00001);

			Assert (Enumerable.Empty<double?> ().Average () == null);
			Assert (double.IsNaN (Enumerable.Empty<double> ().Average ()));
		}

		static void TestSum ()
		{
			Assert (_words.Sum (w => w.Length) == 17);
			Assert (_words.Select (w => w.Length).Sum () == 17);
			try
			{
				new[] { int.MaxValue, 1 }.Sum ();
				throw new Exception ("Sum should have thrown an OverflowException");
			}
			catch (OverflowException) { }

			Assert (Enumerable.Empty<int> ().Sum () == 0);
		}

		static void TestCount ()
		{
			Assert (_words.Count () == 4);
			Assert (_words.Count (w => w.Contains ('n')) == 2);
		}

		static void TestMin()
		{
			Assert (_words.Min () == "brown");
			Assert (_words.Min (w => w.Length) == 3);
			Assert (Enumerable.Empty<int?> ().Min () == null);
			Assert (Enumerable.Empty<int> ().Min (i => (int?)i) == null);
		}

		static void TestMax()
		{
			Assert (_words.Max () == "the");
			Assert (_words.Max (w => w.Length) == 5);
			Assert (Enumerable.Empty<int?> ().Max () == null);
			Assert (Enumerable.Empty<int> ().Max (i => (int?)i) == null);
		}

		static void TestConverters ()
		{
			Assert (_words.ToList ().Count == 4);
			Assert (_words.ToArray ().Length == 4);

			Assert (Enumerable.SequenceEqual (_words.ToList(), _words));
			Assert (Enumerable.SequenceEqual (_words.ToArray (), _words));

			var dic = _words.ToDictionary (w => w, w => w.Length);
			Assert (dic.Count == 4);

			var query = from string w in new System.Collections.ArrayList (_words.ToArray()) select w;
			Assert (Enumerable.SequenceEqual (query, _words));
		}

		static void TestElement ()
		{
			Assert (_words.First() == "the");
			Assert (_words.FirstOrDefault () == "the");
			Assert (Enumerable.Empty<int> ().FirstOrDefault () == 0);

			Assert (_words.Last () == "lynx");
			Assert (_words.LastOrDefault () == "lynx");
			Assert (Enumerable.Empty<int> ().LastOrDefault () == 0);

			Assert (_words.First (w => w.Length == 5) == "quick");
			Assert (_words.Last (w => w.Length == 5) == "brown");

			Assert (_words.Take (1).Single () == "the");
			Assert (_words.Single (w => w.Length == 4) == "lynx");

			try { _words.Single (); throw new Exception ("Should not happen"); }
			catch (InvalidOperationException) { }

			try { Enumerable.Empty<int>().Single (); throw new Exception ("Should not happen"); }
			catch (InvalidOperationException) { }

			try { Enumerable.Empty<int> ().First (); throw new Exception ("Should not happen"); }
			catch (InvalidOperationException) { }

			try { Enumerable.Empty<int> ().Last (); throw new Exception ("Should not happen"); }
			catch (InvalidOperationException) { }

			Assert (_words.ElementAt (0) == "the");
			Assert (_words.ElementAt (1) == "quick");
		}

		static void TestFiltering ()
		{
			Assert (Enumerable.SequenceEqual (_words, _words.Concat (_words).Distinct()));

			Assert (Enumerable.SequenceEqual (_words, _words.Skip (0)));
			Assert (_words.Skip (1).First () == "quick");
			Assert (_words.SkipWhile (w => w.Length == 3).First() == "quick");

			Assert (_words.Take (2).Last () == "quick");
			Assert (_words.TakeWhile (w => w.Length == 3).Single () == "the");

			Assert (_words.Where (w => w.Length == 5).Count () == 2);
		}

		static void TestGeneration ()
		{
			Assert (Enumerable.SequenceEqual (Enumerable.Range (4, 3), new int [] { 4, 5, 6 }));
			Assert (Enumerable.SequenceEqual (Enumerable.Repeat (4, 3), new int [] { 4, 4, 4 }));
		}

		static void TestGrouping ()
		{
			var byLength =
				from word in _words
				group word by word.Length;

			Assert (byLength.Count () == 3);
			Assert (byLength.First().Single() == "the");
			Assert (byLength.Skip(1).First ().First () == "quick");
			Assert (byLength.Skip (1).First ().Last () == "brown");
			Assert (byLength.Last().Single() == "lynx");
		}

		static void TestJoining ()
		{
			PropertyInfo [] stringProps = typeof (string).GetProperties ();
			PropertyInfo [] builderProps = typeof (StringBuilder).GetProperties ();

			var query =
				from s in stringProps
				join b in builderProps
					on new { s.Name, s.PropertyType } equals new { b.Name, b.PropertyType }
				select new
				{
					s.Name,
					s.PropertyType,
					StringToken = s.MetadataToken,
					StringBuilderToken = b.MetadataToken
				};

			Assert (query.Count () == 2);
			Assert (query.First ().Name == "Chars");
			Assert (query.First ().PropertyType == typeof (char));
		}

		static void TestMisc ()
		{
			Assert (!Enumerable.SequenceEqual (_words, _words.Reverse ()));
			Assert (Enumerable.SequenceEqual (_words, _words.Reverse ().Reverse ()));

			Assert (Enumerable.Empty<int> ().DefaultIfEmpty ().Single () == 0);
			Assert (Enumerable.Empty<int> ().DefaultIfEmpty (-1).Single () == -1);
		}

		static void TestOrdering ()
		{
			Assert (Enumerable.SequenceEqual (_words.OrderBy (w => w), "brown lynx quick the".Split ()));
			Assert (Enumerable.SequenceEqual (_words.OrderByDescending (w => w), "the quick lynx brown".Split ()));
			Assert (Enumerable.SequenceEqual (_words.OrderByDescending (w => w.Length).ThenBy (w => w),
				"brown quick lynx the".Split ()));
		}

		static void TestProjection ()
		{
			Assert (Enumerable.SequenceEqual (_words.Select (w => w.Length), new int [] { 3, 5, 5, 4 }));

			var charQuery =
				from word in _words
				from c in word
				orderby c
				select char.ToUpper (c);

			Assert (charQuery.Count () == 17);
			Assert (charQuery.First () == 'B');
		}

		static void TestQuantifiers ()
		{
			Assert (_words.Contains ("lynx"));
			Assert (!_words.Contains ("fox"));

			Assert (_words.Any (w => w == "brown"));
			Assert (!_words.Any (w => w == "fox"));

			Assert (_words.All (w => w.Length < 6));
			Assert (!_words.All (w => w.Length < 5));
		}

		static void TestSet ()
		{
			Assert (_words.Concat (_words).Count () == 8);
			Assert (Enumerable.SequenceEqual (_words, _words.Union (_words)));

			Assert (_words.Intersect ("brown".Split ()).Single () == "brown");
			Assert (_words.Intersect ("fox".Split ()).Count () == 0);

			Assert (_words.Except ("the brown lynx".Split ()).Single () == "quick");
		}
	}
}
