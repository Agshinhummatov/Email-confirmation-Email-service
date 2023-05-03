using EntityFramework_Slider.Services.Interfaces;
using EntityFramework_Slider.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace EntityFramework_Slider.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly IFooterService _footerService;
        public FooterViewComponent(IFooterService footerService)
        {
            _footerService = footerService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.FromResult(View( new FooterVM { Socials = await _footerService.GetSocials(), Settings =  _footerService.GetSettingDatas()}));
        }
    }
}
