using MicroondASP_Net.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;


namespace MicroondASP_Net.Controllers
{
    public class MicrowaveCreateController : Controller
    {
        private const int DefaultSymbol = 0;
        private const int MinimumTime = 1;
        private const int MinimumPower = 1;
        private const string AbsolutePath = "C:\\Users\\Shadoso\\source\\repos\\MicroondASP-Net\\MicroondASP-Net\\App_Data\\microwaveModes.json";

        public ActionResult Index(int symbolIndex = DefaultSymbol)
        {
            var microwaveSymbols = LoadMicrowaveSymbols();
            var symbol = microwaveSymbols.Available[symbolIndex];
            ViewBag.SymbolsIndex = microwaveSymbols.Available.Select((x, index) => new SelectListItem { Value = index.ToString(), Text = x }).ToList();

            var microwaveCreate = new MicrowaveCreate
            {
                Name = string.Empty,
                Description = string.Empty,
                Warning = string.Empty,
                Deletable = true,
                Updatable = true,
                SymbolId = symbolIndex,
            };

            return View(microwaveCreate);
        }

        [HttpPost]
        public ActionResult Create(MicrowaveCreate microwaveCreate)
        {
            var microwaveSymbolsOnError = LoadMicrowaveSymbols();

            // Criando uma lista de itens de seleção para os símbolos disponíveis qquando dá erro
            ViewBag.SymbolsIndex = microwaveSymbolsOnError.Available.Select((text, index) => new SelectListItem { Value = index.ToString(), Text = text }).ToList();
            if (MinimumPower > microwaveCreate.Power)
            {
                ViewBag.ErrorMessage = $"Potência inválida: {microwaveCreate.Power}<br>Potência deve ser maior que {MinimumPower}.";
                return View("Index");
            }

            if (MinimumTime > microwaveCreate.Time)
            {
                ViewBag.ErrorMessage = $"Tempo inválido: {microwaveCreate.Time}<br>Tempo deve ser maior que {MinimumTime}.";
                return View("Index");
            }
            var saving = SaveMicrowavePrograms(microwaveCreate, microwaveCreate.SymbolId);

            var microwaveSymbols = LoadMicrowaveSymbols();

            // Criando uma lista de itens de seleção para os símbolos disponíveis
            ViewBag.SymbolsIndex = microwaveSymbols.Available.Select((text, index) => new SelectListItem { Value = index.ToString(), Text = text }).ToList();

            var newMicrowaveCreate = new MicrowaveCreate();

            return RedirectToAction("Index");
        }
        public MicrowaveSymbols LoadMicrowaveSymbols()
        {
            string text;
            var fileStream = new FileStream(AbsolutePath, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                text = streamReader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<MicrowaveSymbols>(text);
        }

        public MicrowaveSave SaveMicrowavePrograms(MicrowaveCreate microwaveCreate, int symbolIndex)
        {
            string text;
            var fileStream = new FileStream(AbsolutePath, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream))
            {
                text = streamReader.ReadToEnd();
            }
            var microwaveSave = JsonConvert.DeserializeObject<MicrowaveSave>(text);
            string newSymbol = microwaveSave.Available[symbolIndex];
            microwaveSave.Available.RemoveAt(symbolIndex);
            var newProgram = new Program
            {
                Name = microwaveCreate.Name,
                Description = microwaveCreate.Description,
                Time = microwaveCreate.Time,
                Power = microwaveCreate.Power,
                Warning = microwaveCreate.Warning,
                Symbol = newSymbol,
                Deletable = microwaveCreate.Deletable,
                Updatable = microwaveCreate.Updatable,
                Customizable = microwaveCreate.Customizable
            };

            microwaveSave.Programs.Add(newProgram);

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            };
            text = JsonConvert.SerializeObject(microwaveSave, settings);
            using (var outputFileStream = new FileStream(AbsolutePath, FileMode.Create, FileAccess.Write))
            using (var streamWriter = new StreamWriter(outputFileStream))
            {
                streamWriter.Write(text);
                streamWriter.Flush();
            };
            return null;
        }
    }
}