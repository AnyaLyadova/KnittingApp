namespace KnittingApp.Services
{
    public interface ILoopsReaderService
    {
        public Task<(List<string>, List<string>)> GetCurrentString(Guid readerId);

        public Task<int> GetCurrentIndex(Guid readerId);
        public Task<int> GetProgress(Guid readerId);

        public Task SetSpentTime(Guid readerId, TimeSpan time);

        public Task<TimeSpan> GetSpentTime(Guid readerId);
    }
}
