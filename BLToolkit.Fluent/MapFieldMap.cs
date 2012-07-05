using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BLToolkit.Reflection;
using BLToolkit.Reflection.Extension;

namespace BLToolkit.Fluent
{
	/// <summary>
	/// Fluent settings for field
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TR"></typeparam>
	public class MapFieldMap<T, TR> : FluentMap<T>
	{
		private readonly Expression<Func<T, TR>> _prop;

		public MapFieldMap(TypeExtension owner, Expression<Func<T, TR>> prop)
			: base(owner)
		{
			_prop = prop;
		}

		/// <summary>
		/// PrimaryKeyAttribute
		/// </summary>
		/// <typeparam name="TR"></typeparam>
		/// <param name="order"></param>
		/// <returns></returns>
		public MapFieldMap<T, TR> PrimaryKey(int order = -1)
		{
			var member = GetMemberExtension(_prop);
			member.Attributes.Add(Attributes.PrimaryKey.Order, Convert.ToString(order));
			return this;
		}

		/// <summary>
		/// NonUpdatableAttribute
		/// </summary>
		/// <returns></returns>
		public MapFieldMap<T, TR> NonUpdatable()
		{
			var member = GetMemberExtension(_prop);
			member.Attributes.Add(Attributes.NonUpdatable, ToString(true));
			return this;
		}

		/// <summary>
		/// IdentityAttribute
		/// </summary>
		/// <typeparam name="TR"></typeparam>
		/// <returns></returns>
		public MapFieldMap<T, TR> Identity()
		{
			var member = GetMemberExtension(_prop);
			member.Attributes.Add(Attributes.Identity, ToString(true));
			return this;
		}

		/// <summary>
		/// SqlIgnoreAttribute
		/// </summary>
		/// <param name="ignore"></param>
		/// <returns></returns>
		public MapFieldMap<T, TR> SqlIgnore(bool ignore = true)
		{
			var member = GetMemberExtension(_prop);
			member.Attributes.Add(Attributes.SqlIgnore.Ignore, ToString(ignore));
			return this;
		}

		/// <summary>
		/// MapIgnoreAttribute
		/// </summary>
		/// <param name="ignore"></param>
		/// <returns></returns>
		public MapFieldMap<T, TR> MapIgnore(bool ignore = true)
		{
			var member = GetMemberExtension(_prop);
			member.Attributes.Add(Attributes.MapIgnore.Ignore, ToString(ignore));
			return this;
		}

		/// <summary>
		/// TrimmableAttribute
		/// </summary>
		/// <returns></returns>
		public MapFieldMap<T, TR> Trimmable()
		{
			var member = GetMemberExtension(_prop);
			member.Attributes.Add(Attributes.Trimmable, ToString(true));
			return this;
		}

		/// <summary>
		/// MapValueAttribute
		/// </summary>
		/// <typeparam name="TV"> </typeparam>
		/// <param name="origValue"></param>
		/// <param name="value"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public MapFieldMap<T, TR> MapValue<TV>(TR origValue, TV value, params TV[] values)
		{
			var member = GetMemberExtension(_prop);
			FillMapValueExtension(member.Attributes, origValue, value, values);
			return this;
		}

		/// <summary>
		/// DefaultValueAttribute
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public MapFieldMap<T, TR> DefaultValue(TR value)
		{
#warning need test
			var member = GetMemberExtension(_prop);
			member.Attributes.Add(Attributes.DefaultValue, Convert.ToString(value));
			return this;
		}

		/// <summary>
		/// NullableAttribute
		/// </summary>
		/// <param name="isNullable"></param>
		/// <returns></returns>
		public MapFieldMap<T, TR> Nullable(bool isNullable = true)
		{
#warning need test
			var member = GetMemberExtension(_prop);
			member.Attributes.Add(Attributes.Nullable.IsNullable, ToString(isNullable));
			return this;
		}

		/// <summary>
		/// NullValueAttribute
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public MapFieldMap<T, TR> NullValue(TR value)
		{
#warning need test
			var member = GetMemberExtension(_prop);
			member.Attributes.Add(Attributes.NullValue, Convert.ToString(value));
			return this;
		}

		/// <summary>
		/// AssociationAttribute
		/// </summary>
		/// <typeparam name="TRt"></typeparam>
		/// <param name="canBeNull"></param>
		/// <param name="thisKey"></param>
		/// <param name="thisKeys"></param>
		/// <returns></returns>
		public AssociationMap<TR, TRt> Association<TRt>(bool canBeNull, Expression<Func<T, TRt>> thisKey, params Expression<Func<T, TRt>>[] thisKeys)
		{
			var keys = new List<Expression<Func<T, TRt>>>(thisKeys);
			keys.Insert(0, thisKey);
			return new AssociationMap<TR, TRt>(this, _prop, canBeNull, keys);
		}

		/// <summary>
		/// AssociationAttribute
		/// </summary>
		/// <typeparam name="TRt"></typeparam>
		/// <param name="thisKey"></param>
		/// <param name="thisKeys"></param>
		/// <returns></returns>
		public AssociationMap<TR, TRt> Association<TRt>(Expression<Func<T, TRt>> thisKey, params Expression<Func<T, TRt>>[] thisKeys)
		{
			return Association(true, thisKey, thisKeys);
		}

		public MapFieldMap<T, TR> Relation(string slaveIndex = null, string masterIndex = null)
		{
			return Relation(new[] { slaveIndex }, new[] { masterIndex });
		}

		public MapFieldMap<T, TR> Relation(string[] slaveIndex, string[] masterIndex)
		{
			slaveIndex = (slaveIndex ?? new string[0]).Where(i => !string.IsNullOrEmpty(i)).ToArray();
			masterIndex = (masterIndex ?? new string[0]).Where(i => !string.IsNullOrEmpty(i)).ToArray();

			Type destinationType = typeof(TR);
			if (TypeHelper.IsSameOrParent(typeof(IEnumerable), destinationType))
			{
				destinationType = destinationType.GetGenericArguments().Single();
			}
			var member = GetMemberExtension(_prop);
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

			return this;
		}

		private void FillRelationIndex(string[] index, AttributeExtension attributeExtension, string indexName)
		{
			if (index.Any())
			{
				AttributeExtensionCollection collection = new AttributeExtensionCollection();
				foreach (var s in index)
				{
					var ae = new AttributeExtension();
					ae.Values.Add(TypeExtension.AttrName.Name, s);
					collection.Add(ae);
				}
				attributeExtension.Attributes.Add(indexName, collection);
			}
		}
	}
}