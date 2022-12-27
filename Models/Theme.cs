using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace genetrix.Models
{
    public class Theme
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public bool HasImage { get; set; }
        public bool HasFooter{ get; set; }
        public bool HasLink{ get; set; }
        public Unit TitlePositionLeft { get; set; }
        public Unit FooterPositionLeft { get; set; }
        public Unit FooterPositionTop{ get; set; }
        public Unit TitlePositionTop { get; set; }
        public Unit ImageHeigth { get; set; }
        public Unit ImageWidth { get; set; }
        public Position LinkPosition { get; set; }
        public Position ImagePosition { get; set; }
        public string Html { get; set; }
        public string Libelle { get; set; }
    }

    public enum Position
    {
        Top,
        Left,
        Rigth,
        Bottom,
        Center,
        CenterTop,
        CenterLeft,
        CenterRigth,
        CenterBottom,
        LeftBottom,
        RigthBottom,
        LeftTop,
        RigthTop,
        Fill
    }
}