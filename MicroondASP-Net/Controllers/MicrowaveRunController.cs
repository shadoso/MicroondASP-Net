using MicroondASP_Net.Models;
using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Linq;

namespace MicroondASP_Net.Controllers
{
    public class MicrowaveRunController : Controller
    {
        private const int MinimumPower = 1;
        private const int MaximumPower = 10;

        private const int MinimumTime = 1;
        private const int MaximumTime = 120;
        private const int DefaultMode = 0;
        private const int Minute = 60;

        private const string AbsolutePath = "C:\\Users\\Shadoso\\source\\repos\\MicroondASP-Net\\MicroondASP-Net\\App_Data\\microwaveModes.json";

        public ActionResult Index(int programIndex = DefaultMode)
        {

            var microwaveRunModes = LoadMicrowaveRunModes();
            var program = microwaveRunModes.Programs[programIndex];

            ViewBag.ProgramNames = microwaveRunModes.Programs.Select((text, index) => new SelectListItem { Value = index.ToString(), Text = text.Name }).ToList();
            ViewBag.ErrorMessage = string.Empty;

            var microwaveRun = new MicrowaveRun
            {
                ProgramId = programIndex,
                Name = program.Name,
                Description = program.Description,
                Time = program.Time,
                Power = program.Power,
                Warning = program.Warning,
                Symbol = program.Symbol,
                Deletable = program.Deletable,
                Customizable = program.Customizable
            };

            return View(microwaveRun);
        }

        [HttpPost]
        public ActionResult Start(MicrowaveRun microwaveRun)
        {
            var microwaveRunModes = LoadMicrowaveRunModes();

            var program = microwaveRunModes.Programs[microwaveRun.ProgramId];

            // Estes campos estão desabilitados, então devemos carregá-los novamente
            if (!program.Customizable)
            {
                microwaveRun.Time = program.Time;
                microwaveRun.Power = program.Power;
            }
            microwaveRun.Description = program.Description;
            microwaveRun.Warning = program.Warning;
            microwaveRun.Symbol = program.Symbol;

            ViewBag.ProgramNames = microwaveRunModes.Programs.Select((text, index) => new SelectListItem { Value = index.ToString(), Text = text.Name }).ToList();
            
            if (!microwaveRun.Deletable && microwaveRun.Customizable)
            {
                if (MinimumTime > microwaveRun.Time || MaximumTime < microwaveRun.Time)
                {
                    ViewBag.ErrorMessage = $"Tempo inválido: {microwaveRun.Time}<br>Tempo deve estar entre {MinimumTime} segundo e {MaximumTime} segundos.";
                    return View("Index", microwaveRun);
                }
                if (MinimumPower > microwaveRun.Power || MaximumPower < microwaveRun.Power)
                {
                    ViewBag.ErrorMessage = $"Potência inválida: {microwaveRun.Power}<br>Potência deve estar entre {MinimumPower} e {MaximumPower}.";
                    return View("Index", microwaveRun);
                }
            }
            ViewBag.RunningMicrowave = $"Rodando: {microwaveRun.Time / Minute:D2}:{microwaveRun.Time % Minute:D2}";

            return View("Index", microwaveRun);
        }

        [HttpPost]
        public ActionResult Pause(MicrowaveRun microwaveRun)
        {
            var microwaveRunModes = LoadMicrowaveRunModes();

            var program = microwaveRunModes.Programs[microwaveRun.ProgramId];
            if (!program.Customizable)
            {
                microwaveRun.Time = program.Time;
                microwaveRun.Power = program.Power;
            }
            microwaveRun.Description = program.Description;
            microwaveRun.Warning = program.Warning;
            microwaveRun.Symbol = program.Symbol;

            ViewBag.ProgramNames = microwaveRunModes.Programs.Select((text, index) => new SelectListItem { Value = index.ToString(), Text = text.Name }).ToList();
            ViewBag.RunningMicrowave = "Pausado";

            return View("Index", microwaveRun);
        }

        public MicrowaveModes LoadMicrowaveRunModes()
        {
            string text;
            var fileStream = new FileStream(AbsolutePath, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                text = streamReader.ReadToEnd();
            }
            
            return JsonConvert.DeserializeObject<MicrowaveModes>(text);
        }
    }
}