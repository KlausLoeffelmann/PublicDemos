namespace LayoutTests.App.Carrier;

public partial class LazyContainerControl : CarrierContainerBase
{
    public LazyContainerControl()
    {
        InitializeComponent();
        PopulateFacts();
    }

    private void PopulateFacts()
    {
        sampleListView.BeginUpdate();
        try
        {
            sampleListView.Items.Clear();
            foreach (var fact in Facts.PickRandom(20))
            {
                var item = new ListViewItem(fact.Number.ToString());
                item.SubItems.Add(fact.Type);
                item.SubItems.Add(fact.Text);
                sampleListView.Items.Add(item);
            }
        }
        finally
        {
            sampleListView.EndUpdate();
        }
    }
}
