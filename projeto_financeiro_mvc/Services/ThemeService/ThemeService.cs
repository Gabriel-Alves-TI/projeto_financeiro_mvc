using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using projeto_financeiro_mvc.Data;
using projeto_financeiro_mvc.Models;
using projeto_financeiro_mvc.Models.Enums;

namespace projeto_financeiro_mvc.Services.ThemeService
{
    public class ThemeService : IThemeInterface
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAcessor;
        public ThemeService(AppDbContext context, IHttpContextAccessor httpContextAcessor)
        {
            _context = context;
            _httpContextAcessor = httpContextAcessor;
        }

        public Theme GetCurrentTheme()
        {
            var themeClaim = _httpContextAcessor.HttpContext?
                .User?
                .FindFirst("Theme")?
                .Value;

            return Enum.TryParse(themeClaim, out Theme theme)
                ? theme
                : Theme.Vapor;
        }

        public async Task<ResponseModel<Theme>> ChangeTheme(int usuarioId, Theme theme)
        {
            var response = new ResponseModel<Theme>();

            try
            {
                var usuarioDb = _context.Usuarios
                    .Include(u => u.Preferences)
                    .FirstOrDefault(u => u.Id == usuarioId);
                
                if (usuarioDb == null)
                {
                    response.Mensagem = "Usuário não localizado.";
                    response.Status = false;
                    return response;
                }

                if (!Enum.IsDefined(typeof(Theme), theme))
                {
                    response.Mensagem = "Selecione um tema para fazer a alteração.";
                    response.Status = false;
                    return response;
                }

                usuarioDb.Preferences.Theme = theme;

                await _context.SaveChangesAsync();

                response.Mensagem = "Tema alterado com sucesso. Faça login novamente para atualizar o novo layout.";
                return response;
            }
            catch (Exception)
            {
                response.Mensagem = "Erro ao alterar o tema.";
                response.Status = false;
                return response;
            }
        }
    }
}