using System;
using System.Linq;
using BLToolkit.Data;
using BLToolkit.Data.Linq;
using BLToolkit.DataAccess;
using BLToolkit.Fluent.Test.MockDataBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLToolkit.Fluent.Test
{
	/// <summary>
	/// Тестирование FluentMap
	/// </summary>
	[TestClass]
	public class FluentMapTest
	{
		//private SQLiteConnection _conn;
		//private DbManager _db;

		[TestInitialize]
		public void Initialize()
		{
			DbManager.AddDataProvider(typeof(MockDataProvider));
			//DbManager.AddDataProvider(typeof(SQLiteDataProvider));
			//_conn = new SQLiteConnection("data source=:memory:");
			//_db = new DbManager(_conn);
			//FluentConfig.Configure(_db, FluentConfig.GetMapingFromAssemblyOf<FluentMapTest>());
		}

		//[TestCleanup]
		//public void Cleanup()
		//{
		//    _db.Dispose();
		//    _conn.Dispose();
		//}

		/// <summary>
		/// TableName mapping
		/// </summary>
		[TestMethod]
		public void ShouldMapTableName()
		{
			// db config
			var conn = new MockDb()
				.NewReader("Field1")
					.NewRow(1);

			using (conn)
			using (var db = new DbManager(conn))
			{
				// fluent config
				new FluentMap<TableNameDbo>()
					.TableName("TableNameDboT1")
					.MapTo(db);

				// when
				db.GetTable<TableNameDbo>().ToArray();

				// then
				conn.Commands[0]
					.Assert().AreTable("TableNameDboT1", "Fail mapping");
			}
		}

		/// <summary>
		/// MapField mapping
		/// </summary>
		[TestMethod]
		public void ShouldMapField()
		{
			// db config
			var conn = new MockDb()
				.NewNonQuery();

			using (conn)
			using (var db = new DbManager(conn))
			{
				// fluent config
				new FluentMap<MapFieldDbo>()
					.MapField(_ => _.Field1, "f1")
					.MapTo(db);

				// when
				db.GetTable<MapFieldDbo>().Insert(() => new MapFieldDbo { Field1 = 1 });

				// then
				conn.Commands[0]
					.Assert().AreField("f1", "Fail mapping");
			}
		}

		/// <summary>
		/// PrimaryKey mapping
		/// </summary>
		[TestMethod]
		public void ShouldMapPrimaryKey()
		{
			// db config
			var conn = new MockDb()
				.NewReader("Field1", "Field2")
					.NewRow(1, 2);

			using (conn)
			using (var db = new DbManager(conn))
			{
				// fluent config
				new FluentMap<PrimaryKeyDbo>()
					.PrimaryKey(_ => _.Field2)
					.MapTo(db);

				// when
				AssertExceptionEx.AreNotException<Exception>(() => new SqlQuery<PrimaryKeyDbo>(db).SelectByKey(1)
					, "Fail query");

				// then
				Assert.AreEqual(1, conn.Commands[0].Parameters.Count, "Fail params");
				conn.Assert().AreAll("Not query");
			}
		}

		/// <summary>
		/// NonUpdatable use
		/// </summary>
		[TestMethod]
		public void ShouldMapNonUpdatable()
		{
			// db config
			var conn = new MockDb()
				.NewNonQuery();

			using (conn)
			using (var db = new DbManager(conn))
			{
				// fluent config
				new FluentMap<NonUpdatableDbo>()
					.NonUpdatable(_ => _.Field1)
					.MapTo(db);

				// when
				new SqlQuery<NonUpdatableDbo>(db).Insert(new NonUpdatableDbo { Field1 = 10, Field2 = 1 });

				// then
				Assert.AreEqual(1, conn.Commands[0].Parameters.Count, "Fail params");
			}
		}

		/// <summary>
		/// SqlIgnore mapping on insert
		/// </summary>
		[TestMethod]
		public void ShouldMapSqlIgnoreInsert()
		{
			// db config
			var conn = new MockDb()
				.NewNonQuery();

			using (conn)
			using (var db = new DbManager(conn))
			{
				// fluent config
				new FluentMap<SqlIgnoreInsertDbo>()
					.SqlIgnore(_ => _.Field2)
					.MapTo(db);

				// when / then
				new SqlQuery<SqlIgnoreInsertDbo>(db).Insert(new SqlIgnoreInsertDbo { Field1 = 20, Field2 = 2 });
				AssertExceptionEx.AreException<LinqException>(
					() => db.GetTable<SqlIgnoreInsertDbo>().Insert(() => new SqlIgnoreInsertDbo { Field1 = 10, Field2 = 1 })
					, "Fail for linq");

				// then
				conn.Commands[0]
					.Assert().AreNotField("Field2", "Field exists");
				Assert.AreEqual(1, conn.Commands[0].Parameters.Count, "Fail params");
			}
		}

		/// <summary>
		/// SqlIgnore mapping on select
		/// </summary>
		[TestMethod]
		public void ShouldMapSqlIgnoreSelect()
		{
			// db config
			var conn = new MockDb()
				.NewReader("Field1")
					.NewRow(10);

			using (conn)
			using (var db = new DbManager(conn))
			{
				// fluent config
				new FluentMap<SqlIgnoreSelectDbo>()
					.SqlIgnore(_ => _.Field2)
					.MapTo(db);

				var table = db.GetTable<SqlIgnoreSelectDbo>();

				// when
				(from t in table where t.Field1 == 10 select t).First();

				// then
				conn.Commands[0]
					.Assert().AreNotField("Field2", "Field exists");
			}
		}

		/// <summary>
		/// MapIgnore mapping on insert
		/// </summary>
		[TestMethod]
		public void ShouldMapIgnoreInsert()
		{
			// db config
			var conn = new MockDb()
				.NewNonQuery();

			using (conn)
			using (var db = new DbManager(conn))
			{
				// fluent config
				new FluentMap<MapIgnoreInsertDbo>()
					.MapIgnore(_ => _.Field2)
					.MapTo(db);

				// when / then
				new SqlQuery<MapIgnoreInsertDbo>(db).Insert(new MapIgnoreInsertDbo { Field1 = 20, Field2 = 2 });

				AssertExceptionEx.AreException<LinqException>(
					() => db.GetTable<MapIgnoreInsertDbo>().Insert(() => new MapIgnoreInsertDbo { Field1 = 10, Field2 = 1 })
					, "Fail for linq");

				// then
				conn.Commands[0]
					.Assert().AreNotField("Field2", "Field exists");
				Assert.AreEqual(1, conn.Commands[0].Parameters.Count, "Fail params");
			}
		}

		/// <summary>
		/// MapIgnore mapping on select
		/// </summary>
		[TestMethod]
		public void ShouldMapIgnoreSelect()
		{
			// db config
			var conn = new MockDb()
				.NewReader("Field1")
					.NewRow(10, 1);

			using (conn)
			using (var db = new DbManager(conn))
			{
				// fluent config
				new FluentMap<MapIgnoreSelectDbo>()
					.MapIgnore(_ => _.Field2)
					.MapTo(db);

				var table = db.GetTable<MapIgnoreSelectDbo>();

				// when
				(from t in table where t.Field1 == 10 select t).First();

				// then
				conn.Commands[0]
					.Assert().AreNotField("Field2", "Field exists");
			}
		}

		/// <summary>
		/// Trimmable mapping
		/// </summary>
		[TestMethod]
		public void ShouldMapTrimmable()
		{
			// db config
			var conn = new MockDb()
				.NewReader("Field1")
					.NewRow("test     ");

			using (conn)
			using (var db = new DbManager(conn))
			{
				// fluent config
				new FluentMap<TrimmableDbo>()
					.Trimmable(_ => _.Field1)
					.MapTo(db);

				var table = db.GetTable<TrimmableDbo>();

				// when
				var dbo = (from t in table select t).First();

				// then
				Assert.AreEqual("test", dbo.Field1, "Not trimmable");
				conn.Assert().AreAll("Not query");
			}
		}

		#region Dbo
		public class TableNameDbo
		{
			public int Field1 { get; set; }
		}
		public class MapFieldDbo
		{
			public int Field1 { get; set; }
		}
		public class PrimaryKeyDbo
		{
			public int Field1 { get; set; }
			public int Field2 { get; set; }
		}
		public class NonUpdatableDbo
		{
			public int Field1 { get; set; }
			public int Field2 { get; set; }
		}
		public class SqlIgnoreInsertDbo
		{
			public int Field1 { get; set; }
			public int Field2 { get; set; }
		}
		public class SqlIgnoreSelectDbo
		{
			public int Field1 { get; set; }
			public int Field2 { get; set; }
		}
		public class MapIgnoreInsertDbo
		{
			public int Field1 { get; set; }
			public int Field2 { get; set; }
		}
		public class MapIgnoreSelectDbo
		{
			public int Field1 { get; set; }
			public int Field2 { get; set; }
		}
		public class TrimmableDbo
		{
			public string Field1 { get; set; }
		}
		#endregion
	}
}