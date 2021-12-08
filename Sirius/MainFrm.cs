using Sirius.Core;

namespace Sirius;

public partial class MainFrm : Form
{
    private readonly PluginManager<ISiriusPlugin> _pluginManager;

    public MainFrm()
    {
        InitializeComponent();
        _pluginManager = new PluginManager<ISiriusPlugin>();
    }

    private void button1_Click(object sender, EventArgs e)
    {
        openPluginDialog.ShowDialog();

        Emulator.Name = "ric";

        var plugin = _pluginManager.InstallPlugin(openPluginDialog.FileName);

        if (plugin == null)
            MessageBox.Show(@"The plugin has not been installed, make sure it implements the correct interface/class.",
                @"Error on plugin install", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}