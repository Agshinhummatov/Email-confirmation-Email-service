using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using EntityFramework_Slider.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EntityFramework_Slider.ViewComponents
{
    public class ExpertViewComponent : ViewComponent
    {
        private readonly IExpertService _expertService;
        public ExpertViewComponent(IExpertService expertService)
        {
            _expertService = expertService;    
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            return await Task.FromResult(View(  new ExpertVM { Experts = await _expertService.GetAll(), ExpertsHeader = await _expertService.GetHeader() }));
        }
    }
}
