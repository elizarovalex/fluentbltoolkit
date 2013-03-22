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
	public partial class MapFieldMap<T, TR> : FluentMap<T>
	{
		private readonly Expression<Func<T, TR>> _prop;

		public MapFieldMap(TypeExtension owner, List<IFluentMap> childs, Expression<Func<T, TR>> prop)
			: base(owner, childs)
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
			return PrimaryKey(_prop, order);
		}

		/// <summary>
		/// NonUpdatableAttribute
		/// </summary>
		/// <returns></returns>
		public MapFieldMap<T, TR> NonUpdatable()
		{
			return NonUpdatable(_prop);
		}

		/// <summary>
		/// IdentityAttribute
		/// </summary>
		/// <typeparam name="TR"></typeparam>
		/// <returns></returns>
		public MapFieldMap<T, TR> Identity()
		{
			return Identity(_prop);
		}

		/// <summary>
		/// SqlIgnoreAttribute
		/// </summary>
		/// <param name="ignore"></param>
		/// <returns></returns>
		public MapFieldMap<T, TR> SqlIgnore(bool ignore = true)
		{
			return SqlIgnore(_prop, ignore);
		}

		/// <summary>
		/// MapIgnoreAttribute
		/// </summary>
		/// <param name="ignore"></param>
		/// <returns></returns>
		public MapFieldMap<T, TR> MapIgnore(bool ignore = true)
		{
			return MapIgnore(_prop, ignore);
		}

		/// <summary>
		/// TrimmableAttribute
		/// </summary>
		/// <returns></returns>
		public MapFieldMap<T, TR> Trimmable()
		{
			return Trimmable(_prop);
		}

		/// <summary>
		/// MapValueAttribute. Applied for select operations. Not applied for update operations
		/// </summary>
		/// <typeparam name="TV"> </typeparam>
		/// <param name="origValue"></param>
		/// <param name="value"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public MapFieldMap<T, TR> MapValue<TV>(TR origValue, TV value, params TV[] values)
		{
			return MapValue(_prop, origValue, value, values);
		}

		/// <summary>
		/// DefaultValueAttribute
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public MapFieldMap<T, TR> DefaultValue(TR value)
		{
			return DefaultValue(_prop, value);
		}

		/// <summary>
		/// NullableAttribute
		/// </summary>
		/// <param name="isNullable"></param>
		/// <returns></returns>
		public MapFieldMap<T, TR> Nullable(bool isNullable = true)
		{
			return Nullable(_prop, isNullable);
		}

		/// <summary>
		/// NullValueAttribute
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public MapFieldMap<T, TR> NullValue(TR value)
		{
			return NullValue(_prop, value);
		}

		/// <summary>
		/// AssociationAttribute
		/// </summary>
		/// <typeparam name="TRt"></typeparam>
		/// <param name="canBeNull"></param>
		/// <param name="thisKey"></param>
		/// <param name="thisKeys"></param>
		/// <returns></returns>
		public AssociationMap<TRt> Association<TRt>(bool canBeNull, Expression<Func<T, TRt>> thisKey, params Expression<Func<T, TRt>>[] thisKeys)
		{
			return Association(_prop, canBeNull, thisKey, thisKeys);
		}

		/// <summary>
		/// AssociationAttribute
		/// </summary>
		/// <typeparam name="TRt"></typeparam>
		/// <param name="thisKey"></param>
		/// <param name="thisKeys"></param>
		/// <returns></returns>
		public AssociationMap<TRt> Association<TRt>(Expression<Func<T, TRt>> thisKey, params Expression<Func<T, TRt>>[] thisKeys)
		{
			return Association(_prop, thisKey, thisKeys);
		}

		private MapFieldMap<T, TR> Association<TRt, TRf, TRo>(bool canBeNull
			, IEnumerable<Expression<Func<T, TRt>>> thisKeys, IEnumerable<Expression<Func<TRf, TRo>>> otherKeys)
		{
			return Association(_prop, canBeNull, thisKeys, otherKeys);
		}

		/// <summary>
		/// RelationAttribute
		/// </summary>
		/// <param name="slaveIndex"></param>
		/// <param name="masterIndex"></param>
		/// <returns></returns>
		public MapFieldMap<T, TR> Relation(string slaveIndex = null, string masterIndex = null)
		{
			return Relation(_prop, slaveIndex, masterIndex);
		}

		/// <summary>
		/// RelationAttribute
		/// </summary>
		/// <param name="slaveIndex"></param>
		/// <param name="masterIndex"></param>
		/// <returns></returns>
		public MapFieldMap<T, TR> Relation(string[] slaveIndex, string[] masterIndex)
		{
			return Relation(_prop, slaveIndex, masterIndex);
		}
	}
}