using System.Collections.ObjectModel;

namespace FinalFrontier
{
    
    
    class ModelMailFolder : VMBase
    {
        public string FolderName
        {
            get { return folderName; }
            set { SetProperty(ref folderName, value); }
        }
        private string folderName;

        public ObservableCollection<ModelMailFolder> Children
        {
            get { return children; }
            set { SetProperty(ref children, value); }
        }
        private ObservableCollection<ModelMailFolder> children = new ObservableCollection<ModelMailFolder>();

        public bool IsChecked
        {
            get { return isChecked; }
            set { SetProperty(ref isChecked, value); 
                  CheckStatus(); }
        }
        private bool isChecked;


        public ModelMailFolder(string name, ModelMailFolder child = null)
        {
            FolderName = name;
            if (child != null) children.Add(child);
        }

        public void AddChild(ModelMailFolder childrenItem)
        {
            Children.Add(childrenItem);
        }

        public override bool Equals(object obj)
        {
            return (obj as ModelMailFolder).FolderName.Equals(FolderName);
        }

        private void CheckStatus()
        {
            foreach (ModelMailFolder child in Children) child.IsChecked = IsChecked;
        }
    }
}
