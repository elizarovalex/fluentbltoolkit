using System;
using System.Linq.Expressions;
using BLToolkit.Data;
using BLToolkit.Data.DataProvider;
using BLToolkit.Mapping;
using BLToolkit.Reflection.Extension;

namespace BLToolkit.Fluent
{
	/// <summary>
	/// Поддержка возможности настройки мапинга из кода без использования аттрибутов
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public partial class FluentMap<T> : IFluentMap
	{
		private readonly TypeExtension _typeExtension;

		/// <summary>
		/// Конструктор
		/// </summary>
		public FluentMap()
		{
			_typeExtension = new TypeExtension { Name = typeof(T).FullName };
		}

		/// <summary>
		/// Мапинг на таблицу. TableNameAttribute
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public FluentMap<T> TableName(string name)
		{
			_typeExtension.Attributes.Add(Attributes.TableName.Name, name);
			return this;
		}

		/// <summary>
		/// Мапинг на таблицу. TableNameAttribute
		/// </summary>
		/// <param name="database"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public FluentMap<T> TableName(string database, string name)
		{
			TableName(name);
			_typeExtension.Attributes.Add(Attributes.TableName.Database, database);
			return this;
		}

		/// <summary>
		/// Мапинг на таблицу. TableNameAttribute
		/// </summary>
		/// <param name="database"></param>
		/// <param name="owner"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public FluentMap<T> TableName(string database, string owner, string name)
		{
			TableName(database, name);
			_typeExtension.Attributes.Add(Attributes.TableName.Owner, owner);
			return this;
		}

		/// <summary>
		/// Мапинг на поле в таблице. MapFieldAttribute
		/// </summary>
		/// <typeparam name="TR"></typeparam>
		/// <param name="prop"></param>
		/// <param name="mapName"></param>
		/// <param name="storage"></param>
		/// <param name="isInheritanceDiscriminator"></param>
		/// <returns></returns>
		public FluentMap<T> MapField<TR>(Expression<Func<T, TR>> prop, string mapName
			, string storage = null, bool? isInheritanceDiscriminator = null)
		{
			var member = GetMemberExtension(prop);
			member.Attributes.Add(Attributes.MapField.MapName, mapName);
			if (null != storage)
			{
				member.Attributes.Add(Attributes.MapField.Storage, storage);
			}
			if (null != isInheritanceDiscriminator)
			{
				member.Attributes.Add(Attributes.MapField.IsInheritanceDiscriminator, Convert.ToString(isInheritanceDiscriminator.Value));
			}
			return this;
		}

		/// <summary>
		/// Пометка поля как первичного. PrimaryKeyAttribute
		/// </summary>
		/// <typeparam name="TR"></typeparam>
		/// <param name="prop"></param>
		/// <param name="order"></param>
		/// <returns></returns>
		public FluentMap<T> PrimaryKey<TR>(Expression<Func<T, TR>> prop, int order = -1)
		{
			var member = GetMemberExtension(prop);
			member.Attributes.Add(Attributes.PrimaryKey.Order, Convert.ToString(order));
			return this;
		}

		/// <summary>
		/// Пометка поля как необновляемого. NonUpdatableAttribute
		/// </summary>
		/// <typeparam name="TR"></typeparam>
		/// <param name="prop"></param>
		/// <returns></returns>
		public FluentMap<T> NonUpdatable<TR>(Expression<Func<T, TR>> prop)
		{
			var member = GetMemberExtension(prop);
			member.Attributes.Add(Attributes.NonUpdatable, Convert.ToString(true));
			return this;

		}

		/// <summary>
		/// Пометка поля как уникального. IdentityAttribute
		/// </summary>
		/// <typeparam name="TR"></typeparam>
		/// <param name="prop"></param>
		/// <returns></returns>
		public FluentMap<T> Identity<TR>(Expression<Func<T, TR>> prop)
		{
			var member = GetMemberExtension(prop);
			member.Attributes.Add(Attributes.Identity, Convert.ToString(true));
			return this;

		}

		/// <summary>
		/// Пометка поля как игнорируемого. SqlIgnoreAttribute
		/// </summary>
		/// <typeparam name="TR"></typeparam>
		/// <param name="prop"></param>
		/// <param name="ignore"></param>
		/// <returns></returns>
		public FluentMap<T> SqlIgnore<TR>(Expression<Func<T, TR>> prop, bool ignore = true)
		{
			var member = GetMemberExtension(prop);
			member.Attributes.Add(Attributes.SqlIgnore.Ignore, Convert.ToString(ignore));
			return this;

		}

		/// <summary>
		/// Получить результат мапинга
		/// </summary>
		/// <returns></returns>
		public TypeExtension Map()
		{
			return _typeExtension;
		}

		/// <summary>
		/// Применить результа мапинга в DbManager
		/// </summary>
		/// <param name="dbManager"></param>
		public void MapTo(DbManager dbManager)
		{
			var ms = dbManager.MappingSchema ?? (dbManager.MappingSchema = Mapping.Map.DefaultSchema);
			MapTo(ms);
		}

		/// <summary>
		/// Применить результа мапинга в DataProviderBase
		/// </summary>
		/// <param name="dataProvider"></param>
		public void MapTo(DataProviderBase dataProvider)
		{
			var ms = dataProvider.MappingSchema ?? (dataProvider.MappingSchema = Mapping.Map.DefaultSchema);
			MapTo(ms);
		}

		/// <summary>
		/// Применить результа мапинга в MappingSchema
		/// </summary>
		/// <param name="mappingSchema"></param>
		public void MapTo(MappingSchema mappingSchema)
		{
			var extensions = mappingSchema.Extensions ?? (mappingSchema.Extensions = new ExtensionList());
			MapTo(extensions);
		}

		/// <summary>
		/// Применить результа мапинга в ExtensionList
		/// </summary>
		/// <param name="extensions"></param>
		public void MapTo(ExtensionList extensions)
		{
			var ext = Map();
			if (extensions.ContainsKey(ext.Name))
			{
				extensions.Remove(ext.Name);
			}
			extensions.Add(ext);
		}

		private MemberExtension GetMemberExtension<TR>(Expression<Func<T, TR>> prop)
		{
			string name = ((MemberExpression)prop.Body).Member.Name;
			MemberExtension member;
			if (!_typeExtension.Members.TryGetValue(name, out member))
			{
				member = new MemberExtension { Name = name };
				_typeExtension.Members.Add(member);
			}
			return member;
		}
	}
}