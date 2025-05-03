using Ardalis.Specification;

namespace AdminLibrary.SharedKernel.Interfaces
{
    public interface ReadRepository<T>: IReadRepositoryBase<T> where T : class
    {
    }
}
