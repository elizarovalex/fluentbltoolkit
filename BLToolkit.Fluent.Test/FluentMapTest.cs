using System;
using System.Data.SQLite;
using System.Linq;
using BLToolkit.Data;
using BLToolkit.Data.DataProvider;
using BLToolkit.Data.Linq;
using BLToolkit.DataAccess;
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
			_db.SetCommand("create table PrimaryKeyDbo (Field1 int, Field2 int)").ExecuteNonQuery();

			new FluentMap<PrimaryKeyDbo>()
				.PrimaryKey(_ => _.Field2)
				.MapTo(_db);

			var table = _db.GetTable<PrimaryKeyDbo>();
			table.Insert(() => new PrimaryKeyDbo { Field1 = 10, Field2 = 1 });
			table.Insert(() => new PrimaryKeyDbo { Field1 = 20, Field2 = 2 });

			var dbo = new SqlQuery<PrimaryKeyDbo>(_db).SelectByKey(1);

			Assert.AreEqual(10, dbo.Field1, "Неверный результат");
		}

		/// <summary>
		/// Должен применяться мапинг необновляемых полей
		/// </summary>
		[TestMethod]
		public void ShouldMapNonUpdatable()
		{
			_db.SetCommand("create table NonUpdatableDbo (Field1 int, Field2 int)").ExecuteNonQuery();

			new FluentMap<NonUpdatableDbo>()
				.NonUpdatable(_ => _.Field1)
				.MapTo(_db);

			new SqlQuery<NonUpdatableDbo>(_db).Insert(new NonUpdatableDbo { Field1 = 10, Field2 = 1 });

			var table = _db.GetTable<NonUpdatableDbo>();
			var dbo = (from t in table where t.Field2 == 1 select t).First();

			Assert.AreNotEqual(10, dbo.Field1, "Поле было обновлено");
			Assert.AreEqual(1, dbo.Field2, "Неверный результат");
		}

		/// <summary>
		/// Поле должно игнорироваться при вставке
		/// </summary>
		[TestMethod]
		public void ShouldMapSqlIgnoreInsert()
		{
			_db.SetCommand("create table SqlIgnoreInsertDbo (Field1 int)").ExecuteNonQuery();

			new FluentMap<SqlIgnoreInsertDbo>()
				.SqlIgnore(_ => _.Field2)
				.MapTo(_db);


			AssertEx.AreNotException<Exception>(
				() => new SqlQuery<SqlIgnoreInsertDbo>(_db).Insert(new SqlIgnoreInsertDbo { Field1 = 20, Field2 = 2 })
				, "Поле задействовано");
			AssertEx.AreException<LinqException>(
				() => _db.GetTable<SqlIgnoreInsertDbo>().Insert(() => new SqlIgnoreInsertDbo { Field1 = 10, Field2 = 1 })
				, "Поле доступно в linq");
		}

		/// <summary>
		/// Поле должно игнорироваться при запросе
		/// </summary>
		[TestMethod]
		public void ShouldMapSqlIgnoreSelect()
		{
			_db.SetCommand("create table SqlIgnoreSelectDbo (Field1 int, Field2 int)").ExecuteNonQuery();
			_db.SetCommand("insert into SqlIgnoreSelectDbo (Field1, Field2) values(10,1)").ExecuteNonQuery();

			new FluentMap<SqlIgnoreSelectDbo>()
				.SqlIgnore(_ => _.Field2)
				.MapTo(_db);
			var table = _db.GetTable<SqlIgnoreSelectDbo>();

			var dbo = (from t in table where t.Field1 == 10 select t).First();

			Assert.AreNotEqual(1, dbo.Field2, "Значение считано");
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
	}
}