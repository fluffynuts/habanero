using System;
using System.Collections.Generic;
using System.Text;
using Habanero.Base.Exceptions;

namespace Habanero.Base
{
    /// <summary>
    /// Represents a source from which data is retrieved
    /// </summary>
    public class Source
    {
        private string _name;
        private string _entityName;
        private JoinList _joins;
        private JoinList _inheritanceJoins;
        private bool _isPrepared;

        public Source(string name) : this(name, name)
        {
        }

        public Source(string name, string entityName)
        {
            _joins = new JoinList(this);
            _inheritanceJoins = new JoinList(this);
            _name = name;
            _entityName = entityName;
        }

        /// <summary>
        /// Gets and sets the name of the source
        /// </summary>
        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets and sets the entity name of the source
        /// </summary>
        public virtual string EntityName
        {
            get { return _entityName; }
            set { _entityName = value; }
        }

        /// <summary>
        /// Gets the list of joins that make up the source
        /// </summary>
        public virtual JoinList Joins
        {
            get { return _joins; }
        }

        /// <summary>
        /// Gets the source which is a child of this one, which can
        /// occur where one source inherits from another
        /// </summary>
        public Source ChildSource
        {
            get
            {
                if (Joins.Count == 0) return null;
                return Joins[0].ToSource;
            }
        }

        /// <summary>
        /// Gets the furthermost child
        /// </summary>
        public Source ChildSourceLeaf
        {
            get {
                if (ChildSource != null)  return ChildSource.ChildSourceLeaf;
                return this;
            }
        }

        /// <summary>
        /// Gets the list of joins that create one source through inheritance
        /// </summary>
        public virtual JoinList InheritanceJoins
        {
            get { return _inheritanceJoins; }
        }

        /// <summary>
        /// Gets and sets the value that indicates whether the source has been prepared
        /// </summary>
        public bool IsPrepared
        {
            get { return _isPrepared; }
            set { _isPrepared = value; }
        }

        /// <summary>
        /// Returns this source in a string form
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string toString = Name;
            if (this.Joins.Count >0)
            {
                toString += "." + this.Joins[0].ToSource.ToString();
            }
            return toString;
        }

        /// <summary>
        /// Returns this source in hash code form
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }

        /// <summary>
        /// Indicates whether the given source is equal to this one
        /// </summary>
        /// <param name="obj">The source to compare with this one</param>
        /// <returns>Returns true if equal, false if not</returns>
        public override bool Equals(object obj)
        {
            Source source = obj as Source;
            if (source == null) return false;
            return Object.Equals(_name, source._name);
        }

        /// <summary>
        /// Joins a given source onto this one using an inner join
        /// </summary>
        /// <param name="toSource">The source to join onto this one</param>
        public void JoinToSource(Source toSource)
        {
            Joins.AddNewJoinTo(toSource, JoinType.InnerJoin);
        }

        /// <summary>
        /// Manages a list of joins that make up a <see cref="Source"/>
        /// </summary>
        public class JoinList : List<Join>
        {
            private Source _fromSource;

            public JoinList(Source fromSource)
            {
                if (fromSource == null)
                {
                    throw new ArgumentNullException("fromSource");
                }
                _fromSource = fromSource;
            }

            /// <summary>
            /// Gets the source containing this join list
            /// </summary>
            public Source FromSource
            {
                get { return _fromSource; }
            }

            /// <summary>
            /// Merges a given list of joins into this one
            /// </summary>
            /// <param name="joinListToMerge">The list of joins to add to this one</param>
            public void MergeWith(JoinList joinListToMerge)
            {
                Source fromSourceToMerge = joinListToMerge.FromSource;
                if (!_fromSource.Equals(fromSourceToMerge))
                    throw new HabaneroDeveloperException(
                        "A source's joins cannot merge with another source's joins " + 
                        "if they do not have the same base source.",
                        "Please check your Source structures. Base Source:" + this +
                        ", Source to merge:" + fromSourceToMerge);
                if (joinListToMerge.Count == 0) return;
                Join inheritanceJoinToMerge = joinListToMerge[0];
                Source toSourceToMerge = inheritanceJoinToMerge.ToSource;
                Join inheritanceJoin = null;
                Source toSource = null;
                if (this.Count > 0)
                {
                    inheritanceJoin = this[0];
                    toSource = inheritanceJoin.ToSource;
                }
                if (!Object.Equals(toSourceToMerge, toSource))
                {
                    toSource = toSourceToMerge;
                    Join newJoin = this.AddNewJoinTo(toSourceToMerge, inheritanceJoinToMerge.JoinType);
                    foreach (Join.JoinField joinField in inheritanceJoinToMerge.JoinFields)
                    {
                        newJoin.JoinFields.Add(joinField);
                    }
                }
                if (toSource != null)
                {
                    toSource.MergeWith(toSourceToMerge);
                }
                
            }

            /// <summary>
            /// Adds a new join from the source containing this join list to the
            /// specified source, using the specified join type
            /// </summary>
            /// <param name="toSource">The source to connect the current source to</param>
            /// <param name="joinType">The type of join to use</param>
            /// <returns>Returns the newly created join</returns>
            public Join AddNewJoinTo(Source toSource, JoinType joinType)
            {
                if (toSource == null) return null;
                bool alreadyExists = this.Exists(delegate(Join join1)
                {
                    return join1.ToSource.Name.Equals(toSource.Name);
                });
                if (alreadyExists) return null;
                Join join = new Join(_fromSource, toSource, joinType);
                this.Add(join);
                return join;
            }
        }

        /// <summary>
        /// Provides a list of join types used to connect sources
        /// </summary>
        public enum JoinType
        {
            /// <summary>
            /// Merges two sources only on the records with they match on some given criteria
            /// </summary>
            InnerJoin,
            /// <summary>
            /// Merges two sources by including all records in the primary (left) source
            /// and only rows in the secondary (right) source that match on some given criteria
            /// </summary>
            LeftJoin
        }

        /// <summary>
        /// Represents a join between sources that allows the multiple sources to
        /// be regarded as one
        /// </summary>
        public class Join
        {
            private Source _fromSource;
            private Source _toSource;
            private List<JoinField> _joinFields = new List<JoinField>( );
            private JoinType _joinType;

            public Join(Source fromSource, Source toSource) : this(fromSource, toSource, Source.JoinType.InnerJoin)
            {
            }

            public Join(Source fromSource, Source toSource, JoinType joinType)
            {
                _fromSource = fromSource;
                _toSource = toSource;
                _joinType = joinType;
            }

            /// <summary>
            /// Gets the primary source from which the join originates
            /// </summary>
            public Source FromSource
            {
                get { return _fromSource; }
            }

            /// <summary>
            /// Gets the source to which this join connects
            /// </summary>
            public Source ToSource
            {
                get { return _toSource; }
            }

            /// <summary>
            /// Gets a list of fields on which the two sources must match
            /// </summary>
            public List<JoinField> JoinFields
            {
                get { return _joinFields; }
            }

            /// <summary>
            /// Gets and sets the type of join used to connect the sources
            /// </summary>
            public JoinType JoinType
            {
                get { return _joinType; }
                set { _joinType = value; }
            }

            /// <summary>
            /// Represents a field on which two sources must match
            /// </summary>
            public class JoinField
            {
                private QueryField _fromField;
                private QueryField _toField;

                public JoinField(QueryField fromField, QueryField toField)
                {
                    _fromField = fromField;
                    _toField = toField;
                }

                /// <summary>
                /// Gets the field in the primary source
                /// </summary>
                public QueryField FromField
                {
                    get { return _fromField; }
                }

                /// <summary>
                /// Gets the field in the secondary source
                /// </summary>
                public QueryField ToField
                {
                    get { return _toField; }
                }
            }

            /// <summary>
            /// Gets the clause used to indicate the type of join
            /// </summary>
            /// <returns></returns>
            public string GetJoinClause()
            {
                switch (JoinType)
                {
                    case JoinType.InnerJoin:
                        return "JOIN";
                    case JoinType.LeftJoin:
                        return "LEFT JOIN";
                }
                return "";
            }
        }

        /// <summary>
        /// Gets the string clause that represents the source from which a join originates
        /// </summary>
        /// <param name="sourcename">The name of the source</param>
        /// <returns></returns>
        public static Source FromString(string sourcename)
        {
            if (String.IsNullOrEmpty(sourcename)) return null;
            string[] sourceParts = sourcename.Split('.');
            if (sourceParts.Length == 1) return new Source(sourcename);
            Source baseSource = new Source(sourceParts[0]);
            Source currentSource = baseSource;
            for (int i = 1; i < sourceParts.Length; i++)
            {
                Source newSource = new Source(sourceParts[i]);
                currentSource.Joins.Add(new Join(currentSource, newSource, Source.JoinType.LeftJoin));
                currentSource = newSource;
            }
            return baseSource;
        }

        /// <summary>
        /// Merges the current source with another specified source
        /// </summary>
        /// <param name="sourceToMerge">The source to merge this one with</param>
        public void MergeWith(Source sourceToMerge)
        {
            if (sourceToMerge == null) return;
            if (String.IsNullOrEmpty(sourceToMerge.Name)) return;
            if (!this.Equals(sourceToMerge))
                throw new HabaneroDeveloperException("A source cannot merge with another source if they do not have the same base source.", 
                        "Please check your Source structures. Base Source:" + this + " source to merge " + sourceToMerge);

            this.InheritanceJoins.MergeWith(sourceToMerge.InheritanceJoins);
            this.Joins.MergeWith(sourceToMerge.Joins);
        }
    }
}