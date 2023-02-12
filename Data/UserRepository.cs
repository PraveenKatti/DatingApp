using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        public DataContext _context { get; }
        public IMapper _mapper { get; }
        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        async Task<AppUser> IUserRepository.GetUserByIdAsync(int id)
        {
             return await _context.Users.FindAsync(id);
        }

        async Task<AppUser> IUserRepository.GetUserByUsernameAsync(string username)
        {
            return await _context.Users
            .Include(p=>p.Photos)
            .SingleOrDefaultAsync(x=>x.UserName == username);
        }

        async Task<IEnumerable<AppUser>> IUserRepository.GetUsersAsync()
        {
            return await _context.Users
            .Include(a=>a.Photos)
            .ToListAsync();
        }

        async Task<bool> IUserRepository.SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        void IUserRepository.Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
             return await _context.Users
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await _context.Users
            .Where(x=>x.UserName == username)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
        }
    }
}