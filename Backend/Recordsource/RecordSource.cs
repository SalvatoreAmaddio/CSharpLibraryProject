using Backend.Controller;
using Backend.Database;
using Backend.Model;
using MvvmHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Recordsource
{
    /// <summary>
    /// This class creates an ObservableRangeCollection that deals with IEnumerable&lt;<see cref="ISQLModel"/>&gt;. As Enumerator it uses a <see cref="INavigator"/>.
    /// see also the <seealso cref="Navigator"/> class.
    /// </summary>
    public class RecordSource : ObservableRangeCollection<ISQLModel>, IParentSource, IChildSource
    {
        INavigator? navigator;
        private List<IChildSource> Children { get; } = [];
        public IAbstractSQLModelController? Controller { get; set; }
        public RecordSource(IEnumerable<ISQLModel> source) : base(source) { }
        public RecordSource() { }

        /// <summary>
        /// Override the default <c>GetEnumerator()</c> method to replace it with a <see cref="INavigator"></see> object./>
        /// </summary>
        /// <returns>An Enumerator object.</returns>
        public new IEnumerator<ISQLModel> GetEnumerator()
        {
            if (navigator != null) 
            {
                int index = navigator.Index;
                navigator = new Navigator(this)
                {
                    Index = index
                };
                return navigator!;
            }
            navigator = new Navigator(this);
            return navigator!;
        }

        /// <summary>
        /// Return the Enumerator as an <see cref="INavigator"/> object.
        /// </summary>
        /// <returns>A <see cref="INavigator"/> object.</returns>
        public INavigator Navigate() => (INavigator)GetEnumerator();

        /// <summary>
        /// It takes an IAsyncEnumerable, converts it to a List and returns a RecordSource object.
        /// </summary>
        /// <param name="source"> An IAsyncEnumerable&lt;ISQLModel></param>
        /// <returns>Task&lt;RecordSource></returns>
        public static async Task<RecordSource> CreateFromAsyncList(IAsyncEnumerable<ISQLModel> source) =>
        new RecordSource(await source.ToListAsync());

        public void AddChild(IChildSource child) => Children.Add(child);   

        public void NotifyChildren(CRUD crud, ISQLModel model)
        {
            if (crud == CRUD.UPDATE) return;
            foreach (var child in Children) child.Update(crud, model);
        }

        public void Update(CRUD crud, ISQLModel model)
        {
            switch (crud)
            {
                case CRUD.INSERT:
                    Add(model);
                    Controller?.GoLast();
                    break;
                //case CRUD.UPDATE: NO NEEDED BECAUSE OBJECTS ARE REFERENCED.
                //    int index = IndexOf(model);
                //    if (index >= 0) this[index] = model;
                //    break;
                case CRUD.DELETE:
                    bool removed = Remove(model);
                    if (!removed) break;
                    if (navigator == null) throw new Exception("NO SOURCE");
                    if (navigator.BOF && !navigator.NoRecords) Controller?.GoFirst();
                    else Controller?.GoPrevious();
                    break;
            }
            //run filter
        }

        public void RemoveChild(IChildSource child) => Children.Remove(child);

        /// <summary>
        /// Return a the position of the records within the RecordSource.
        /// </summary>
        /// <returns>A string.</returns>
        public string RecordPositionDisplayer() 
        {
            if (navigator == null) return "NO SOURCE";
            return true switch
            {
                true when navigator.NoRecords => "NO RECORDS",
                true when navigator.IsNewRecord => "New Record",
                _ => $"Record {navigator?.RecNum} of {navigator?.RecordCount}",
            };
        }
    }
}