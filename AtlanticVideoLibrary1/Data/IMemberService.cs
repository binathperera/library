using AtlanticVideoLibrary1.Pages;

namespace AtlanticVideoLibrary1.Data
{
    public interface IMemberService
    {
        List<Member> GetMembers();
        List<Member> Search(String s);
        Member GetMember(String id);
        bool Update(Member m);
        bool Add(Member m);
        bool Delete(String id);
        String GenerateId();
    }
}
