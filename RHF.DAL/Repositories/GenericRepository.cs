using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using RHF.DAL.Interfaces;
using RHF.Shared;

namespace RHF.DAL.Repositories
{
    public abstract class GenericRepository<T> : IGenericRepository<T, Expression<Func<T, bool>>> where T : class
    {
        private readonly RhfDbContext _context;
        private readonly IOperationResult _logResult;
        public GenericRepository(RhfDbContext context, IOperationResult logResult)
        {
            _context = context;
            _logResult = logResult;
        }
        public virtual IOperationResult Add(T t) 
        {
            try
            {
                _context.Add(t);
                 _context.SaveChanges();
                _logResult.AddMessage($"{t.GetType()} persisted successfully!!");
                _logResult.Success = true;
                return _logResult;
            }
            catch (System.Exception e)
            {  
                _logResult.AddMessage($"{e.ToString()} persisted successfully!!");           
                throw;
            }
        }

        public virtual async Task<IList<T>>  GetAllObjects()
        {
            try
            {
                return await _context.Set<T>().ToListAsync();
            }
            catch (System.Exception)
            {              
                throw;
            }
        }

        public async Task<T> GetObjectById(Expression<Func<T, bool>> condition)
        {
            try
            {
                 return await _context.Set<T>()
                    .Where(condition).FirstOrDefaultAsync();
            }
            catch (System.Exception)
            {              
                throw;
            }
        }

        public IOperationResult RemoveObjectById(Expression<Func<T, bool>> condition)
        {
            try
            {
                var obj= _context.Set<T>()
                 .Where(condition).FirstOrDefault();
                 _context.Remove(obj);
                
                _context.SaveChanges();
                _logResult.AddMessage($"{obj.GetType()} removed successfully!!");
                _logResult.Success = true;
                return _logResult;
            }
            catch (System.Exception e)
            {  
              _logResult.AddMessage($"error: {e.ToString()}");                   
                throw;
            }
        }

        public IOperationResult Update(T t, Expression<Func<T, bool>> condition)
        {
           try
           {
               var obj= _context.Set<T>()
                 .Where(condition).FirstOrDefault(); 
                _context.Update(obj);
                _context.SaveChanges();
                _logResult.AddMessage($"{obj.GetType()} updated successfully!!");
                _logResult.Success = true;
                return _logResult;
           }
           catch (System.Exception e)
           {
               _logResult.AddMessage($"error: {e.ToString()}");                              
               throw;
           }
        }
    }
}