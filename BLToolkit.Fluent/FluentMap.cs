using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BLToolkit.Data;
using BLToolkit.Data.DataProvider;
using BLToolkit.Mapping;
using BLToolkit.Reflection;
using BLToolkit.Reflection.Extension;

namespace BLToolkit.Fluent
{
	/// <summary>
	/// FluentSettings
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public partial class FluentMap<T> : IFluentMap
	{
		private readonly TypeExtension _typeExtension;
		private const string MemberNameSeparator = ".";

		/// <summary>
		/// ctor
		/// </summary>
		public FluentMap()
			: this(new TypeExtension { Name = typeof(T).FullName })
		{
		}

		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="typeExtension"></param>
		protected FluentMap(TypeExtension typeExtension)
		{
			_typeExtension = typeExtension;
		}

		/// <summary>
		/// TableNameAttribute
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public FluentMap<T> TableName(string name)
		{
			_typeExtension.Attributes.Add(Attributes.TableName.Name, name);
			return this;
		}

		/// <summary>
		/// TableNameAttribute
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
		/// TableNameAttribute
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
		/// MapFieldAttribute
		/// </summary>
		/// <typeparam name="TR"></typeparam>
		/// <param name="prop"></param>
		/// <param name="isInheritanceDiscriminator"></param>
		/// <returns></returns>
		public MapFieldMap<T, TR> MapField<TR>(Expression<Func<T, TR>> prop, bool isInheritanceDiscriminator)
		{
			return MapField(prop, null, null, isInheritanceDiscriminator);
		}

		/// <summary>
		/// MapFieldAttribute
		/// </summary>
		/// <typeparam name="TR"></typeparam>
		/// <param name="prop"></param>
		/// <param name="mapName"></param>
		/// <param name="storage"></param>
		/// <param name="isInheritanceDiscriminator"></param>
		/// <returns></returns>
		public MapFieldMap<T, TR> MapField<TR>(Expression<Func<T, TR>> prop, string mapName = null, string storage = null, bool? isInheritanceDiscriminator = null)
		{
			string name = GetExprName(prop);
			if (name.Contains(MemberNameSeparator))
			{
				MapFieldOnType(name, mapName);
			}
			else
			{
				MapFieldOnField(name, mapName, storage, isInheritanceDiscriminator);
			}
			return new MapFieldMap<T, TR>(_typeExtension, prop);
		}

		private void MapFieldOnType(string origName, string mapName)
		{
			AttributeExtensionCollection attrs;
			if (!_typeExtension.Attributes.TryGetValue(Attributes.MapField.Name, out attrs))
			{
				attrs = new AttributeExtensionCollection();
				_typeExtension.Attributes.Add(Attributes.MapField.Name, attrs);
			}
			var attributeExtension = new AttributeExtension();
			attributeExtension.Values.Add(Attributes.MapField.OrigName, origName);
			attributeExtension.Values.Add(Attributes.MapField.MapName, mapName);
			attrs.Add(attributeExtension);
		}

		private void MapFieldOnField(string origName, string mapName, string storage, bool? isInheritanceDiscriminator)
		{
			var member = GetMemberExtension(origName);
			if (!string.IsNullOrEmpty(mapName))
			{
				member.Attributes.Add(Attributes.MapField.Name, mapName);
			}
			if (null != storage)
			{
				member.Attributes.Add(Attributes.MapField.Storage, storage);
			}
			if (null != isInheritanceDiscriminator)
			{
				member.Attributes.Add(Attributes.MapField.IsInheritanceDiscriminator, ToString(isInheritanceDiscriminator.Value));
			}
		}

		/// <summary>
		/// MapValueAttribute
		/// </summary>
		/// <typeparam name="TV"></typeparam>
		/// <param name="origValue"></param>
		/// <param name="value"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public FluentMap<T> MapValue<TV>(Enum origValue, TV value, params TV[] values)
		{
			MemberExtension member;
			var name = Enum.GetName(origValue.GetType(), origValue);
			if (!_typeExtension.Members.TryGetValue(name, out member))
			{
				member = new MemberExtension { Name = name };
				_typeExtension.Members.Add(member);
			}
			FillMapValueExtension(member.Attributes, origValue, value, values);
			return this;
		}

		/// <summary>
		/// MapValueAttribute
		/// </summary>
		/// <typeparam name="TV"></typeparam>
		/// <param name="origValue"></param>
		/// <param name="value"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public FluentMap<T> MapValue<TV>(object origValue, TV value, params TV[] values)
		{
			FillMapValueExtension(_typeExtension.Attributes, origValue, value, values);
			return this;
		}

		/// <summary>
		/// InheritanceMappingAttribute
		/// </summary>
		/// <typeparam name="TC"></typeparam>
		/// <param name="code"></param>
		/// <returns></returns>
		public FluentMap<T> InheritanceMapping<TC>(object code)
		{
			return InheritanceMapping<TC>(code, null);
		}

		/// <summary>
		/// InheritanceMappingAttribute
		/// </summary>
		/// <typeparam name="TC"></typeparam>
		/// <param name="isDefault"></param>
		/// <returns></returns>
		public FluentMap<T> InheritanceMapping<TC>(bool isDefault)
		{
			return InheritanceMapping<TC>(null, isDefault);
		}

		/// <summary>
		/// InheritanceMappingAttribute
		/// </summary>
		/// <typeparam name="TC"></typeparam>
		/// <param name="code"></param>
		/// <param name="isDefault"></param>
		/// <returns></returns>
		public FluentMap<T> InheritanceMapping<TC>(object code, bool? isDefault)
		{
			AttributeExtensionCollection extList;
			if (!_typeExtension.Attributes.TryGetValue(Attributes.InheritanceMapping.Name, out extList))
			{
				extList = new AttributeExtensionCollection();
				_typeExtension.Attributes.Add(Attributes.InheritanceMapping.Name, extList);
			}
			var attr = new AttributeExtension();
			attr.Values.Add(Attributes.InheritanceMapping.Type, typeof(TC).AssemblyQualifiedName);
			if (null != code)
			{
				attr.Values.Add(Attributes.InheritanceMapping.Code, code);
			}
			if (null != isDefault)
			{
				attr.Values.Add(Attributes.InheritanceMapping.IsDefault, isDefault.Value);
			}
			extList.Add(attr);
			return this;
		}

		protected void FillMapValueExtension<TR, TV>(AttributeNameCollection attributeCollection, TR origValue, TV value, TV[] values)
		{
			AttributeExtensionCollection list;
			if (!attributeCollection.TryGetValue(Attributes.MapValue.Name, out list))
			{
				list = new AttributeExtensionCollection();
				attributeCollection.Add(Attributes.MapValue.Name, list);
			}

			var allValues = new List<TV>(values);
			allValues.Insert(0, value);
			var tvFullName = typeof(TV).FullName;

			foreach (var val in allValues)
			{
				var attributeExtension = new AttributeExtension();
				attributeExtension.Values.Add(Attributes.MapValue.OrigValue, origValue);
				attributeExtension.Values.Add(TypeExtension.ValueName.Value, Convert.ToString(val));
				attributeExtension.Values.Add(TypeExtension.ValueName.Value + TypeExtension.ValueName.TypePostfix, tvFullName);
				list.Add(attributeExtension);
			}
		}

		/// <summary>
		/// Fluent settings result
		/// </summary>
		/// <returns></returns>
		public TypeExtension Map()
		{
			return _typeExtension;
		}

		/// <summary>
		/// Apply fluent settings to DbManager
		/// </summary>
		/// <param name="dbManager"></param>
		public void MapTo(DbManager dbManager)
		{
			var ms = dbManager.MappingSchema ?? (dbManager.MappingSchema = Mapping.Map.DefaultSchema);
			MapTo(ms);
		}

		/// <summary>
		/// Apply fluent settings to DataProviderBase
		/// </summary>
		/// <param name="dataProvider"></param>
		public void MapTo(DataProviderBase dataProvider)
		{
			var ms = dataProvider.MappingSchema ?? (dataProvider.MappingSchema = Mapping.Map.DefaultSchema);
			MapTo(ms);
		}

		/// <summary>
		/// Apply fluent settings to MappingSchema
		/// </summary>
		/// <param name="mappingSchema"></param>
		public void MapTo(MappingSchema mappingSchema)
		{
			var extensions = mappingSchema.Extensions ?? (mappingSchema.Extensions = new ExtensionList());
			MapTo(extensions);
		}

		/// <summary>
		/// Apply fluent settings to ExtensionList
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

		protected MemberExtension GetMemberExtension<TR>(Expression<Func<T, TR>> prop)
		{
			string name = GetExprName(prop);
			return GetMemberExtension(name);
		}

		protected MemberExtension GetMemberExtension(string name)
		{
			MemberExtension member;
			if (!_typeExtension.Members.TryGetValue(name, out member))
			{
				member = new MemberExtension { Name = name };
				_typeExtension.Members.Add(member);
			}
			return member;
		}

		private string GetExprName<TT, TR>(Expression<Func<TT, TR>> prop)
		{
			string result = null;
			var memberExpression = prop.Body as MemberExpression;
			while (null != memberExpression)
			{
				result = null == result ? "" : MemberNameSeparator + result;
				result = memberExpression.Member.Name + result;
				memberExpression = memberExpression.Expression as MemberExpression;
			}
			if (null == result)
			{
				throw new ArgumentException("Fail member access expression.");
			}
			return result;
		}

		/// <summary>
		/// Invert for BLToolkit.Reflection.Extension.TypeExtension.ToBoolean()
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected string ToString(bool value)
		{
			return Convert.ToString(value);
		}
	}
}