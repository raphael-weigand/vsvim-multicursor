using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace VsVim.MultiCursor
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("VsVim MultiCursor", "Native caret above/below", "1.0")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [Guid(PackageGuidString)]
    public sealed class MultiCursorPackage : AsyncPackage
    {
        public const string PackageGuidString = "59028fbd-0f65-4d00-8dbc-9c3f2c28920a";
        private static readonly Guid CommandSet = new Guid("bc3d4ab4-a4c6-4f4a-b7d4-8b1236ed7080");

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            var menu = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (menu == null) return;
            menu.AddCommand(new MenuCommand((s, e) => AddCaret(1), new CommandID(CommandSet, 0x0100)));
            menu.AddCommand(new MenuCommand((s, e) => AddCaret(-1), new CommandID(CommandSet, 0x0101)));
        }

        private void AddCaret(int direction)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var textManager = GetService(typeof(SVsTextManager)) as IVsTextManager;
            if (textManager == null ||
                ErrorHandler.Failed(textManager.GetActiveView(1, null, out IVsTextView activeView)) ||
                activeView == null) return;
            var componentModel = GetService(typeof(SComponentModel)) as IComponentModel;
            var adapters = componentModel?.GetService<IVsEditorAdaptersFactoryService>();
            IWpfTextView view = adapters?.GetWpfTextView(activeView);
            if (view == null || view.IsClosed) return;
            CaretOperations.AddAdjacent(view, direction);
        }
    }
}
