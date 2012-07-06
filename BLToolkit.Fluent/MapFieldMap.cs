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
		public MapFieldMap(TypeExtension owner, Expression<Func<T, TR>> prop)
			: base(owner)
		{
			Prop = prop;
		}

		/// <summary>
		/// Current context
		/// </summary>
		public Expression<Func<T, TR>> Prop { get; private set; }

		/// <summary>
		/// PrimaryKeyAttribute
		/// </summary>
		/// <typeparam name="TR"></typeparam>
		/// <param name="order"></param>
		/// <returns></returns>
		public MapFieldMap<T, TR> PrimaryKey(int order = -1)
		{
			return PrimaryKey(Prop, order);
		}

		/// <summary>
		/// NonUpdatableAttribute
		/// </summary>
		/// <returns></returns>
		public MapFieldMap<T, TR> NonUpdatable()
		{
			return NonUpdatable(Prop);
		}

		/// <summary>
		/// IdentityAttribute
		/// </summary>
		/// <typeparam name="TR"></typeparam>
		/// <returns></returns>
		public MapFieldMap<T, TR> Identity()
		{
			return Identity(Prop);
		}

		/// <summary>
		/// SqlIgnoreAttribute
		/// </summary>
		/// <param name="ignore"></param>
		/// <returns></returns>
		public MapFieldMap<T, TR> SqlIgnore(bool ignore = true)
		{
			return SqlIgnore(Prop, ignore);
		}

		/// <summary>
		/// MapIgnoreAttribute
		/// </summary>
		/// <param name="ignore"></param>
		/// <returns></returns>
		public MapFieldMap<T, TR> MapIgnore(bool ignore = true)
		{
			return MapIgnore(Prop, ignore);
		}

		/// <summary>
		/// TrimmableAttribute
		/// </summary>
		/// <returns></returns>
		public MapFieldMap<T, TR> Trimmable()
		{
			return Trimmable(Prop);
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
			return MapValue(Prop, origValue, value, values);
		}

		/// <summary>
		/// DefaultValueAttribute
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public MapFieldMap<T, TR> DefaultValue(TR value)
		{
			return DefaultValue(Prop, value);
		}

		/// <summary>
		/// NullableAttribute
		/// </summary>
		/// <param name="isNullable"></param>
		/// <returns></returns>
		public MapFieldMap<T, TR> Nullable(bool isNullable = true)
		{
			return Nullable(Prop, isNullable);
		}

		/// <summary>
		/// NullValueAttribute
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public MapFieldMap<T, TR> NullValue(TR value)
		{
			return NullValue(Prop, value);
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
			return Association(Prop, canBeNull, thisKey, thisKeys);
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
			return Association(Prop, thisKey, thisKeys);
		}

		/// <summary>
		/// RelationAttribute
		/// </summary>
		/// <param name="slaveIndex"></param>
		/// <param name="masterIndex"></param>
		/// <returns></returns>
		public MapFieldMap<T, TR> Relation(string slaveIndex = null, string masterIndex = null)
		{
			return Relation(Prop, slaveIndex, masterIndex);
		}

		/// <summary>
		/// RelationAttribute
		/// </summary>
		/// <param name="slaveIndex"></param>
		/// <param name="masterIndex"></param>
		/// <returns></returns>
		public MapFieldMap<T, TR> Relation(string[] slaveIndex, string[] masterIndex)
		{
			return Relation(Prop, slaveIndex, masterIndex);
		}
	}
}