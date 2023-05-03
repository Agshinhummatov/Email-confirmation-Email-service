
using EntityFramework_Slider.Services;
using EntityFramework_Slider.Services.Interfaces;
using EntityFramework_Slider.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EntityFramework_Slider.ViewComponents
{
    public class BlogViewComponent : ViewComponent
    {
        private readonly IBlogService _blogService;
        public BlogViewComponent(IBlogService blogService)
        {
            _blogService = blogService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //var blogs = await _blogService.GetAll();

            //var ourBlogHeader = await _blogService.GetOurBlogHeader();

            //var model = new BlogVM
            //{
            //    Blogs = blogs,
            //    OurBlogHeader = ourBlogHeader
            //};     //qisa olsun deye awagidaki formani istifade edirik

            return await Task.FromResult(View(new BlogVM { Blogs = await _blogService.GetAll(), OurBlogHeader = await _blogService.GetOurBlogHeader() }));
            //taskin result-i olaraq view-a blog-u gonder (bele oxunur)
        }


        //InvokeAsync-na -- argument gondermek olur.Seyfede componenti cagiranda Invoke(icine arqument) gonderik
        //public async Task<IViewComponentResult> InvokeAsync(int skip)  //comonentde bir nov Index action mentiqidi -InvokeAsync()
        //{
        //    return await Task.FromResult(View(await _context.Blogs.Skip(skip).Take(3).ToListAsync()));       //Invoke bize bucur return edir Task.FromResult
        //}




    }
}
