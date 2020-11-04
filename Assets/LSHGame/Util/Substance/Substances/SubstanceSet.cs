using System.Collections.Generic;

namespace LSHGame.Util
{
    public sealed class SubstanceSet
    {
        public ISubSetQuery SubSetQuery { get; }

        private HashSet<ISubstance> substances = new HashSet<ISubstance>();

        public SubstanceSet(ISubSetQuery subSetQuery)
        {
            SubSetQuery = subSetQuery;
        }

        public SubstanceSet() : this(new SubSetQuery())
        {
        }

        public bool Contains(ISubstance substance)
        {
            return substances.Contains(substance);
        }

        public void Add(ISubstance substance)
        {
            substances.Add(substance);
        }

        public void RecieveDataAndReset(IDataReciever dataReciever)
        {
            foreach (var substance in substances)
            {
                substance.RecieveData(dataReciever);
            }

            substances.Clear();
        }

        public void ExecuteQuery()
        {
            SubSetQuery.ExecuteQuery(this);
        }
    }

    public class SubSetQuery : ISubSetQuery
    {
        private HashSet<ISubSetQueryable> queryables = new HashSet<ISubSetQueryable>();

        public void Add(ISubSetQueryable queryable)
        {
            queryables.Add(queryable);
        }

        public void ExecuteQuery(SubstanceSet substanceSet)
        {
            queryables.RemoveWhere(e => e.ExeAddToSet(substanceSet));
        }

        public void Remove(ISubSetQueryable queryable)
        {
           queryables.Remove(queryable);
        }
    }

    public interface ISubSetQuery
    {
        void Add(ISubSetQueryable executable);

        void Remove(ISubSetQueryable executable);

        void ExecuteQuery(SubstanceSet substanceSet);
    }

    public interface ISubSetQueryable
    {
        /// <summary>
        /// Method should be executed every FixedUpdate by the controller of the DataReciever
        /// </summary>
        /// <returns>Wether the IExecutable should be disposed</returns>
        bool ExeAddToSet(SubstanceSet substanceSet);
    }
}
