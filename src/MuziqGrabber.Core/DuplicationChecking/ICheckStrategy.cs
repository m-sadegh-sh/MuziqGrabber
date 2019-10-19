namespace MuziqGrabber.Core.DuplicationChecking {
    public interface ICheckStrategy {
        bool IsDuplicate(string fileName);
    }
}