using System;
using System.Collections;
using System.Linq;
using BLToolkit.Reflection;
using BLToolkit.Reflection.Extension;

namespace BLToolkit.Fluent
{
	public partial class FluentMap<T> : IFluentMap
	{
		/// <summary>
		/// Get mapping
		/// </summary>
		/// <returns></returns>
		ExtensionList IFluentMap.Map()
		{
			return Map();
		}

		/// <summary>
		/// TableNameAttribute
		/// </summary>
		/// <param name="database"></param>
		/// <param name="owner"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		void IFluentMap.TableName(string database, string owner, string name)
		{
			if (null != database)
			{
				_typeExtension.Attributes.Add(Attributes.TableName.Database, database);
			}
			if (null != owner)
			{
				_typeExtension.Attributes.Add(Attributes.TableName.Owner, owner);
			}
			if (null != name)
			{
				_typeExtension.Attributes.Add(Attributes.TableName.Name, name);
			}
			EachChilds(m => m.TableName(database, owner, name));
		}

		/// <summary>
		/// Map to ExtensionList
		/// </summary>
		/// <param name="extensions"></param>
		void IFluentMap.MapTo(ExtensionList extensions)
		{
			MapTo(extensions);
		}

		/// <summary>
		/// Maps the field.
		/// </summary>
		/// <param name="propName">Name of the prop.</param>
		/// <param name="mapName">Name of the map.</param>
		/// <param name="storage">The storage.</param>
		/// <param name="isInheritanceDiscriminator">The is inheritance discriminator.</param>
		/// <returns></returns>
		void IFluentMap.MapField(string propName, string mapName, string storage, bool? isInheritanceDiscriminator)
		{
			if (propName.Contains(MemberNameSeparator))
			{
				MapFieldOnType(propName, mapName);
			}
			else
			{
				MapFieldOnField(propName, mapName, storage, isInheritanceDiscriminator);
			}
			EachChilds(m => m.MapField(propName, mapName, storage, isInheritanceDiscriminator));
		}

		/// <summary>
		/// Primaries the key.
		/// </summary>
		/// <param name="propName">Name of the prop.</param>
		/// <param name="order">The order.</param>
		void IFluentMap.PrimaryKey(string propName, int order)
		{
			var member = GetMemberExtension(propName);
			member.Attributes.Add(Attributes.PrimaryKey.Order, Convert.ToString(order));
			EachChilds(m => m.PrimaryKey(propName, order));
		}

		/// <summary>
		/// Nons the updatable.
		/// </summary>
		/// <param name="propName">Name of the prop.</param>
		void IFluentMap.NonUpdatable(string propName)
		{
			var member = GetMemberExtension(propName);
			member.Attributes.Add(Attributes.NonUpdatable, ToString(true));
			EachChilds(m => m.NonUpdatable(propName));
		}

		/// <summary>
		/// Identities the specified prop name.
		/// </summary>
		/// <param name="propName">Name of the prop.</param>
		void IFluentMap.Identity(string propName)
		{
			var member = GetMemberExtension(propName);
			member.Attributes.Add(Attributes.Identity, ToString(true));
			EachChilds(m => m.Identity(propName));
		}

		/// <summary>
		/// SQLs the ignore.
		/// </summary>
		/// <param name="propName">Name of the prop.</param>
		/// <param name="ignore">if set to <c>true</c> [ignore].</param>
		void IFluentMap.SqlIgnore(string propName, bool ignore)
		{
			var member = GetMemberExtension(propName);
			member.Attributes.Add(Attributes.SqlIgnore.Ignore, ToString(ignore));
			EachChilds(m => m.SqlIgnore(propName, ignore));
		}

		/// <summary>
		/// Maps the ignore.
		/// </summary>
		/// <param name="propName">Name of the prop.</param>
		/// <param name="ignore">if set to <c>true</c> [ignore].</param>
		void IFluentMap.MapIgnore(string propName, bool ignore)
		{
			var member = GetMemberExtension(propName);
			member.Attributes.Add(Attributes.MapIgnore.Ignore, ToString(ignore));
			EachChilds(m => m.MapIgnore(propName, ignore));
		}

		/// <summary>
		/// Trimmables the specified prop name.
		/// </summary>
		/// <param name="propName">Name of the prop.</param>
		void IFluentMap.Trimmable(string propName)
		{
			var member = GetMemberExtension(propName);
			member.Attributes.Add(Attributes.Trimmable, ToString(true));
			EachChilds(m => m.Trimmable(propName));
		}

		/// <summary>
		/// Maps the value.
		/// </summary>
		/// <typeparam name="TR">The type of the R.</typeparam>
		/// <typeparam name="TV">The type of the V.</typeparam>
		/// <param name="propName">Name of the prop.</param>
		/// <param name="origValue">The orig value.</param>
		/// <param name="value">The value.</param>
		/// <param name="values">The values.</param>
		void IFluentMap.MapValue<TR, TV>(string propName, TR origValue, TV value, TV[] values)
		{
			var member = GetMemberExtension(propName);
			FillMapValueExtension(member.Attributes, origValue, value, values);
			EachChilds(m => m.MapValue(propName, origValue, value, values));
		}

		/// <summary>
		/// Maps the value.
		/// </summary>
		/// <typeparam name="TV">The type of the V.</typeparam>
		/// <param name="origValue">The orig value.</param>
		/// <param name="value">The value.</param>
		/// <param name="values">The values.</param>
		void IFluentMap.MapValue<TV>(Enum origValue, TV value, TV[] values)
		{
			MemberExtension member;
			var name = Enum.GetName(origValue.GetType(), origValue);
			if (!_typeExtension.Members.TryGetValue(name, out member))
			{
				member = new MemberExtension { Name = name };
				_typeExtension.Members.Add(member);
			}
			FillMapValueExtension(member.Attributes, origValue, value, values);
			EachChilds(m => m.MapValue(origValue, value, values));
		}

		/// <summary>
		/// Maps the value.
		/// </summary>
		/// <typeparam name="TV">The type of the V.</typeparam>
		/// <param name="origValue">The orig value.</param>
		/// <param name="value">The value.</param>
		/// <param name="values">The values.</param>
		void IFluentMap.MapValue<TV>(object origValue, TV value, TV[] values)
		{
			FillMapValueExtension(_typeExtension.Attributes, origValue, value, values);
			EachChilds(m => m.MapValue(origValue, value, values));
		}

		/// <summary>
		/// Defauls the value.
		/// </summary>
		/// <typeparam name="TR">The type of the R.</typeparam>
		/// <param name="propName">Name of the prop.</param>
		/// <param name="value">The value.</param>
		void IFluentMap.DefaulValue<TR>(string propName, TR value)
		{
			var member = GetMemberExtension(propName);
			member.Attributes.Add(Attributes.DefaultValue, Convert.ToString(value));
			EachChilds(m => m.DefaulValue(propName, value));
		}

        /// <summary>
        /// MemberMapper
		/// </summary>
		/// <typeparam name="TR">The type of the R.</typeparam>
		/// <param name="propName">Name of the prop.</param>
		/// <param name="value">The value.</param>
        void IFluentMap.MemberMapper<TR>(string propName, Type memberType, Type memberMapperType)
		{
            var member = GetMemberExtension(propName);
            FillMemberMapperExtension(member.Attributes, memberType, memberMapperType);
            EachChilds(m => m.MemberMapper<TR>(propName, memberType, memberMapperType));
		}
        
		/// <summary>
		/// Nullables the specified prop name.
		/// </summary>
		/// <param name="propName">Name of the prop.</param>
		/// <param name="isNullable">if set to <c>true</c> [is nullable].</param>
		void IFluentMap.Nullable(string propName, bool isNullable)
		{
			var member = GetMemberExtension(propName);
			member.Attributes.Add(Attributes.Nullable.IsNullable, ToString(isNullable));
			EachChilds(m => m.Nullable(propName, isNullable));
		}

		/// <summary>
		/// Nulls the value.
		/// </summary>
		/// <typeparam name="TR">The type of the R.</typeparam>
		/// <param name="propName">Name of the prop.</param>
		/// <param name="value">The value.</param>
		void IFluentMap.NullValue<TR>(string propName, TR value)
		{
			var member = GetMemberExtension(propName);
			member.Attributes.Add(Attributes.NullValue, Convert.ToString(value));
			EachChilds(m => m.NullValue(propName, value));
		}

		/// <summary>
		/// Associations the specified prop name.
		/// </summary>
		/// <param name="propName">Name of the prop.</param>
		/// <param name="canBeNull">if set to <c>true</c> [can be null].</param>
		/// <param name="thisKeys">The this keys.</param>
		/// <param name="otherKeys">The other keys.</param>
		void IFluentMap.Association(string propName, bool canBeNull, string thisKeys, string otherKeys)
		{
			var member = GetMemberExtension(propName);
			AttributeExtensionCollection attrs;
			if (!member.Attributes.TryGetValue(TypeExtension.NodeName.Association, out attrs))
			{
				attrs = new AttributeExtensionCollection();
				member.Attributes.Add(TypeExtension.NodeName.Association, attrs);
			}
			attrs.Clear();
			var attributeExtension = new AttributeExtension();
			attributeExtension.Values.Add(Attributes.Association.ThisKey, thisKeys);
			attributeExtension.Values.Add(Attributes.Association.OtherKey, otherKeys);
			attributeExtension.Values.Add(Attributes.Association.Storage, ToString(canBeNull));
			attrs.Add(attributeExtension);
			EachChilds(m => m.Association(propName, canBeNull, thisKeys, otherKeys));
		}

		/// <summary>
		/// Relations the specified prop name.
		/// </summary>
		/// <param name="propName">Name of the prop.</param>
		/// <param name="destinationType">Type of the destination.</param>
		/// <param name="slaveIndex">Index of the slave.</param>
		/// <param name="masterIndex">Index of the master.</param>
		void IFluentMap.Relation(string propName, Type destinationType, string[] slaveIndex, string[] masterIndex)
		{
			if (TypeHelper.IsSameOrParent(typeof(IEnumerable), destinationType))
			{
				destinationType = destinationType.GetGenericArguments().Single();
			}
			var member = GetMemberExtension(propName);
			AttributeExtensionCollection attrs;
			if (!member.Attributes.TryGetValue(TypeExtension.NodeName.Relation, out attrs))
			{
				attrs = new AttributeExtensionCollection();
				member.Attributes.Add(TypeExtension.NodeName.Relation, attrs);
			}
			attrs.Clear();
			var attributeExtension = new AttributeExtension();
			attributeExtension.Values.Add(TypeExtension.AttrName.DestinationType, destinationType.AssemblyQualifiedName);
			attrs.Add(attributeExtension);

			FillRelationIndex(slaveIndex, attributeExtension, TypeExtension.NodeName.SlaveIndex);
			FillRelationIndex(masterIndex, attributeExtension, TypeExtension.NodeName.MasterIndex);
			EachChilds(m => m.Relation(propName, destinationType, slaveIndex, masterIndex));
		}

		/// <summary>
		/// Inheritances the mapping.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="code">The code.</param>
		/// <param name="isDefault">The is default.</param>
		void IFluentMap.InheritanceMapping(Type type, object code, bool? isDefault)
		{
			AttributeExtensionCollection extList;
			if (!_typeExtension.Attributes.TryGetValue(Attributes.InheritanceMapping.Name, out extList))
			{
				extList = new AttributeExtensionCollection();
				_typeExtension.Attributes.Add(Attributes.InheritanceMapping.Name, extList);
			}
			var attr = new AttributeExtension();
			attr.Values.Add(Attributes.InheritanceMapping.Type, type.AssemblyQualifiedName);
			if (null != code)
			{
				attr.Values.Add(Attributes.InheritanceMapping.Code, code);
			}
			if (null != isDefault)
			{
				attr.Values.Add(Attributes.InheritanceMapping.IsDefault, isDefault.Value);
			}
			extList.Add(attr);
			EachChilds(m => m.InheritanceMapping(type,code,isDefault));
		}
	}
}