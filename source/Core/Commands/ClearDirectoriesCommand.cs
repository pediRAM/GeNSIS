namespace GeNSIS.Core.Commands
{
    public class ClearDirectoriesCommand : ACommand
    {
        public ClearDirectoriesCommand(AppDataVM pAppDataViewModel) : base(pAppDataViewModel) { }

        public override bool CanExecute(object parameter)
            => AppDataViewModel.Sections != null && AppDataViewModel.Sections.Count > 0;

        public override void Execute(object parameter)
            => AppDataViewModel.Sections.Clear();
    }
}
