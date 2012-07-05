using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BLToolkit.Reflection.Extension;

namespace BLToolkit.Fluent
{
	public partial class FluentMap<T>
	{
		public class AssociationMap<TR, TRt>
		{
			private readonly FluentMap<T> _owner;
			private readonly Expression<Func<T, TR>> _prop;
			private readonly bool _canBeNull;
			private readonly List<Expression<Func<T, TRt>>> _thisKeys;

			public AssociationMap(FluentMap<T> owner, Expression<Func<T, TR>> prop, bool canBeNull, List<Expression<Func<T, TRt>>> thisKeys)
			{
				_owner = owner;
				_prop = prop;
				_canBeNull = canBeNull;
				_thisKeys = thisKeys;
			}

			public FluentMap<T> ToMany<TRf, TRo>(Expression<Func<TRf, TRo>> otherKey, params Expression<Func<TRf, TRo>>[] otherKeys)
			{
				var keys = new List<Expression<Func<TRf, TRo>>>(otherKeys);
				keys.Insert(0, otherKey);
				return Association(keys);
			}

			public FluentMap<T> ToOne<TRo>(Expression<Func<TR, TRo>> otherKey, params Expression<Func<TR, TRo>>[] otherKeys)
			{
				var keys = new List<Expression<Func<TR, TRo>>>(otherKeys);
				keys.Insert(0, otherKey);
				return Association(keys);
			}

			private FluentMap<T> Association<TRf, TRo>(List<Expression<Func<TRf, TRo>>> otherKeys)
			{
				var member = _owner.GetMemberExtension(_prop);
				AttributeExtensionCollection attrs;
				if (!member.Attributes.TryGetValue(TypeExtension.NodeName.Association, out attrs))
				{
					attrs = new AttributeExtensionCollection();
					member.Attributes.Add(TypeExtension.NodeName.Association, attrs);
				}
				attrs.Clear();
				var attributeExtension = new AttributeExtension();
				attributeExtension.Values.Add(Attributes.Association.ThisKey, KeysToString(_thisKeys));
				attributeExtension.Values.Add(Attributes.Association.OtherKey, KeysToString(otherKeys));
				attributeExtension.Values.Add(Attributes.Association.Storage, _owner.ToString(_canBeNull));
				attrs.Add(attributeExtension);
				return _owner;
			}

			/// <summary>
			/// Reverse on BLToolkit.Mapping.Association.ParseKeys()
			/// </summary>
			/// <typeparam name="T1"></typeparam>
			/// <typeparam name="T2"></typeparam>
			/// <param name="keys"></param>
			/// <returns></returns>
			private string KeysToString<T1, T2>(IEnumerable<Expression<Func<T1, T2>>> keys)
			{
				return keys.Select(k => _owner.GetExprName(k)).Aggregate((s1, s2) => s1 + ", " + s2);
			}
		}
	}
}