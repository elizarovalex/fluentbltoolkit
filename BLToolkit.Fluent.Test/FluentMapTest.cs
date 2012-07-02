using System;
using System.Data.SQLite;
using System.Linq;
using BLToolkit.Data;
using BLToolkit.Data.DataProvider;
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
		private SQLiteConnection _conn;
		private DbManager _db;

		[TestInitialize]
		public void Initialize()
		{
			DbManager.AddDataProvider(typeof(SQLiteDataProvider));
			DbManager.AddDataProvider(typeof(MockDataProvider));
			_conn = new SQLiteConnection("data source=:memory:");
			_db = new DbManager(_conn);
			FluentConfig.Configure(_db, FluentConfig.GetMapingFromAssemblyOf<FluentMapTest>());
		}

		[TestCleanup]
		public void Cleanup()
		{
			_db.Dispose();
			_conn.Dispose();
		}

		/// <summary>
		/// Должен применяться мапинг таблицы
		/// </summary>
		[TestMethod]
		public void ShouldMapTableName()
		{
			_db.SetCommand("create table TableNameDboT1 (Field1 int)").ExecuteNonQuery();

			new FluentMap<TableNameDbo>()
				.TableName("TableNameDboT1")
				.MapTo(_db);

			AssertEx.AreNotException<Exception>(() => _db.GetTable<TableNameDbo>().ToArray()
				, "Нет мапинга");
		}

		/// <summary>
		/// Должен применяться мапинг поля
		/// </summary>
		[TestMethod]
		public void ShouldMapField()
		{
			_db.SetCommand("create table MapFieldDbo (f1 int)").ExecuteNonQuery();

			new FluentMap<MapFieldDbo>()
				.MapField(_ => _.Field1, "f1")
				.MapTo(_db);

			AssertEx.AreNotException<Exception>(() => _db.GetTable<MapFieldDbo>().Insert(() => new MapFieldDbo { Field1 = 1 })
				, "Нет мапинга");
			Assert.AreEqual(1, _db.GetTable<MapFieldDbo>().Count(), "Неверная таблица");
		}

		/// <summary>
		/// Должен применяться мапинг первичного ключа
		/// </summary>
		[TestMethod]
		public void ShouldMapPrimaryKey()
		{
			var conn = new MockDb()
				.NewReader("Field1", "Field2")
					.NewRow(1, 2);

			using (conn)
			using (var db = new DbManager(conn))
			{
				new FluentMap<PrimaryKeyDbo>()
					.PrimaryKey(_ => _.Field2)
					.MapTo(db);

				AssertEx.AreNotException<Exception>(() => new SqlQuery<PrimaryKeyDbo>(db).SelectByKey(1)
					, "Fail query");

				Assert.AreEqual(1, conn.Commands[0].Parameters.Count, "Fail params");
				conn.Verify("Not query");
			}
		}

		/// <summary>
		/// Должен применяться мапинг необновляемых полей
		/// </summary>
		[TestMethod]
		public void ShouldMapNonUpdatable()
		{
			var conn = new MockDb()
				.NewNonQuery();

			using (conn)
			using (var db = new DbManager(conn))
			{
				new FluentMap<NonUpdatableDbo>()
					.NonUpdatable(_ => _.Field1)
					.MapTo(db);

				new SqlQuery<NonUpdatableDbo>(db).Insert(new NonUpdatableDbo { Field1 = 10, Field2 = 1 });

				Assert.AreEqual(1, conn.Commands[0].Parameters.Count, "Fail params");
				conn.Verify("Not query");
			}
		}

		/// <summary>
		/// Поле должно игнорироваться при вставке
		/// </summary>
		[TestMethod]
		public void ShouldMapSqlIgnoreInsert()
		{
			var conn = new MockDb()
				.NewNonQuery();

			using (conn)
			using (var db = new DbManager(conn))
			{
				new FluentMap<SqlIgnoreInsertDbo>()
					.SqlIgnore(_ => _.Field2)
					.MapTo(db);


				new SqlQuery<SqlIgnoreInsertDbo>(db).Insert(new SqlIgnoreInsertDbo { Field1 = 20, Field2 = 2 });
				AssertEx.AreException<LinqException>(
					() => db.GetTable<SqlIgnoreInsertDbo>().Insert(() => new SqlIgnoreInsertDbo { Field1 = 10, Field2 = 1 })
					, "Поле доступно в linq");

				Assert.AreEqual(1, conn.Commands[0].Parameters.Count, "Fail params");
				conn.Verify("Not query");
			}
		}

		/// <summary>
		/// Поле должно игнорироваться при запросе
		/// </summary>
		[TestMethod]
		public void ShouldMapSqlIgnoreSelect()
		{
			var conn = new MockDb()
				.NewReader("Field1")
					.NewRow(10);

			using (conn)
			using (var db = new DbManager(conn))
			{
				new FluentMap<SqlIgnoreSelectDbo>()
					.SqlIgnore(_ => _.Field2)
					.MapTo(db);
				var table = db.GetTable<SqlIgnoreSelectDbo>();

				var dbo = (from t in table where t.Field1 == 10 select t).First();

				Assert.AreNotEqual(1, dbo.Field2, "Значение считано");
				conn.Verify("Not query");
			}
		}

		/// <summary>
		/// Поле должно игнорироваться при вставке
		/// </summary>
		[TestMethod]
		public void ShouldMapIgnoreInsert()
		{
			var conn = new MockDb()
				.NewNonQuery();

			using (conn)
			using (var db = new DbManager(conn))
			{
				new FluentMap<MapIgnoreInsertDbo>()
					.MapIgnore(_ => _.Field2)
					.MapTo(db);

				new SqlQuery<MapIgnoreInsertDbo>(db).Insert(new MapIgnoreInsertDbo { Field1 = 20, Field2 = 2 });

				AssertEx.AreException<LinqException>(
					() => db.GetTable<MapIgnoreInsertDbo>().Insert(() => new MapIgnoreInsertDbo { Field1 = 10, Field2 = 1 })
					, "Поле доступно в linq");

				Assert.AreEqual(1, conn.Commands[0].Parameters.Count, "");
				conn.Verify("Not query");
			}
		}

		/// <summary>
		/// Поле должно игнорироваться при запросе
		/// </summary>
		[TestMethod]
		public void ShouldMapIgnoreSelect()
		{
			var conn = new MockDb()
				.NewReader("Field1", "Field2")
					.NewRow(10, 1);

			using (conn)
			using (var db = new DbManager(conn))
			{
				new FluentMap<MapIgnoreSelectDbo>()
					.MapIgnore(_ => _.Field2)
					.MapTo(db);
				var table = db.GetTable<MapIgnoreSelectDbo>();

				var dbo = (from t in table where t.Field1 == 10 select t).First();

				Assert.AreNotEqual(1, dbo.Field2, "Значение считано");
				conn.Verify("Not query");
			}
		}

		/// <summary>
		/// Test Trimmable
		/// </summary>
		[TestMethod]
		public void ShouldMapTrimmable()
		{
			var conn = new MockDb()
				.NewReader("Field1")
					.NewRow("test     ");

			using (conn)
			using (var db = new DbManager(conn))
			{
				new FluentMap<TrimmableDbo>()
					.Trimmable(_ => _.Field1)
					.MapTo(db);
				var table = db.GetTable<TrimmableDbo>();

				var dbo = (from t in table select t).First();

				Assert.AreEqual("test", dbo.Field1, "Not trimmable");
				conn.Verify("Not query");
			}
		}

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
	}
}