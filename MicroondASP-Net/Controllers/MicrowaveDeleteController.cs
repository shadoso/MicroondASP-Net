using MicroondASP_Net.Models;
using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Linq;

namespace MicroondASP_Net.Controllers
{
    public class MicrowaveDeleteController : Controller
    {
        private const int DefaultProgram = 0;
        private const string AbsolutePath = "C:\\Users\\Shadoso\\source\\repos\\MicroondASP-Net\\MicroondASP-Net\\App_Data\\microwaveModes.json";
        public ActionResult Index(int programDeleteIndex = DefaultProgram)
        {
            var microwaveDeleteModes = LoadMicrowaveDelete();
            var program = microwaveDeleteModes.Programs[programDeleteIndex];

            ViewBag.ProgramDeleteNames = microwaveDeleteModes.Programs.Select((text, index) => new SelectListItem { Value = index.ToString(), Text = text.Name }).ToList();

            var microwaveDelete = new MicrowaveDelete
            {
                ProgramId = programDeleteIndex,
                Symbol = program.Symbol,
                Deletable = program.Deletable,

            };
            return View(microwaveDelete);
        }

        [HttpPost]
        public ActionResult Delete(MicrowaveDelete microwaveDelete)
        {
            var microwaveDeleteModesOnError = LoadMicrowaveDelete();
            var program = microwaveDeleteModesOnError.Programs[microwaveDelete.ProgramId];
            ViewBag.ProgramDeleteNames = microwaveDeleteModesOnError.Programs.Select((text, index) => new SelectListItem { Value = index.ToString(), Text = text.Name }).ToList();
            microwaveDelete.Name = program.Name;
            microwaveDelete.Symbol = program.Symbol;
            microwaveDelete.Deletable = program.Deletable;

            if (!microwaveDelete.Deletable)
            {
                ViewBag.ErrorMessage = $"Você não pode deletar o modo {microwaveDelete.Name} pois ele é pré-definido";
                return View("Index", microwaveDelete);
            }
            var microwaveDeleteModes = LoadMicrowaveDelete();
            var deleting = RemoveMicrowave(microwaveDelete, microwaveDelete.ProgramId);
            ViewBag.ProgramDeleteNames = microwaveDeleteModes.Programs.Select((text, index) => new SelectListItem { Value = index.ToString(), Text = text.Name }).ToList();
            return RedirectToAction("Index");
        }

        public MicrowaveModes LoadMicrowaveDelete()
        {
            string text;
            var fileStream = new FileStream(AbsolutePath, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                text = streamReader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<MicrowaveModes>(text);
        }
        public MicrowaveRemove RemoveMicrowave(MicrowaveDelete microwaveDelete, int programId)
        {
            string text;
            var fileStream = new FileStream(AbsolutePath, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream))
            {
                text = streamReader.ReadToEnd();
            }
            var microwaveRemove = JsonConvert.DeserializeObject<MicrowaveRemove>(text);
            string availableAgain = microwaveDelete.Symbol;
            microwaveRemove.Programs.RemoveAt(programId);
            microwaveRemove.Available.Add(availableAgain);

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
            };
            text = JsonConvert.SerializeObject(microwaveRemove, settings);
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