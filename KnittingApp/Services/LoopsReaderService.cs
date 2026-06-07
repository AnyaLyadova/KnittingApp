using KnittingApp.Extensions;
using KnittingApp.Repository;

namespace KnittingApp.Services
{
    public class LoopsReaderService:ILoopsReaderService
    {
        private readonly ILoopsReaderRepository loopsReaderRepository;

        public LoopsReaderService(ILoopsReaderRepository loopsReaderRepository)
        {
            this.loopsReaderRepository = loopsReaderRepository;;
        }

        public async Task<LoopsReader>CreateLoopsReader(int startIndex, LoopMap loopMap)
        {
            var reader=new LoopsReader(startIndex, loopMap);
            await loopsReaderRepository.CreateLoopsReader(reader.ToModel());
            return reader;
        }

        public async Task<(List<string>, List<string>)> GetCurrentString(Guid readerId)
        {
            var readerModel=await loopsReaderRepository.GetLoopsReader(readerId);
            var reader = readerModel.ToObject();
            return reader.GetCurrentString();
        }

        public async Task MoveNext(Guid readerId)
        {
            var readerModel = await loopsReaderRepository.GetLoopsReader(readerId);
            var reader = readerModel.ToObject();
            reader.GoNext();
            readerModel=reader.ToModel();
            await loopsReaderRepository.UpdateLoopsReader(readerModel);
        }

        public async Task<int> GetCurrentIndex(Guid readerId)
        {
            var readerModel = await loopsReaderRepository.GetLoopsReader(readerId);
            var reader = readerModel.ToObject();
            return reader.GetCurrentIndex();
        }
        public async Task<int> GetProgress(Guid readerId)
        {
            var readerModel = await loopsReaderRepository.GetLoopsReader(readerId);
            var reader = readerModel.ToObject();
            return reader.GetProgress();
        }

        public async Task SetSpentTime(Guid readerId, TimeSpan time)
        {
            var readerModel = await loopsReaderRepository.GetLoopsReader(readerId);
            var reader = readerModel.ToObject();
            reader.SetSpentTime(time);
            await loopsReaderRepository.UpdateLoopsReader(reader.ToModel());
        }

        public async Task<TimeSpan> GetSpentTime(Guid readerId)
        {
            var readerModel = await loopsReaderRepository.GetLoopsReader(readerId);
            var reader = readerModel.ToObject();
            return reader.GetSpentTime();
        }
    }
}
