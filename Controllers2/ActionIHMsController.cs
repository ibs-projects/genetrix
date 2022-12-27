

using genetrix.Models;
using System.Data.Entity;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace genetrix.Controllers
{
    public class ActionIHMsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ActionIHMs
        public async Task<ActionResult> Index()
        {
            return View(await db.Actions.ToListAsync());
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ViewImage(int id)
        {
            var item = db.Actions.Find(id);
            byte[] buffer = item.Icon;
            if (buffer == null) buffer = new byte[10];
            return File(buffer, "image/jpg", string.Format("{0}.jpg", id));
        }

        // GET: ActionIHMs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActionIHM actionIHM = await db.Actions.FindAsync(id);
            if (actionIHM == null)
            {
                return HttpNotFound();
            }
            return View(actionIHM);
        }

        // GET: ActionIHMs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ActionIHMs/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IconName,Recherche,Intitule,Url,Icon,Recherche")] ActionIHM actionIHM, HttpPostedFileBase uploadImage)
        {
            if (ModelState.IsValid)
            {
                if (uploadImage!=null)
                    if (uploadImage.ContentLength > 0)
                    {
                        byte[] imageData = null;
                        using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                        {
                            imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                        }
                        actionIHM.Icon = imageData;
                    }
                //return RedirectToAction("BrowseImages");
                db.Actions.Add(actionIHM);
                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(actionIHM);
        }

        // GET: ActionIHMs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActionIHM actionIHM = await db.Actions.FindAsync(id);
            if (actionIHM == null)
            {
                return HttpNotFound();
            }
            return View(actionIHM);
        }

        // POST: ActionIHMs/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IconName,Recherche,Id,Intitule,Url,Icon,Recherche")] ActionIHM actionIHM)
        {
            if (ModelState.IsValid)
            {
                db.Entry(actionIHM).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(actionIHM);
        }

        // GET: ActionIHMs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActionIHM actionIHM = await db.Actions.FindAsync(id);
            if (actionIHM == null)
            {
                return HttpNotFound();
            }
            return View(actionIHM);
        }

        // POST: ActionIHMs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ActionIHM actionIHM = await db.Actions.FindAsync(id);
            db.Actions.Remove(actionIHM);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
