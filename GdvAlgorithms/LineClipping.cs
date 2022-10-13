namespace GdvAlgorithms;

public static class ClippingWindow
{
    public static double XMax { get; set; }
    public static double XMin { get; set; }
    public static double YMax { get; set; }
    public static double YMin { get; set; }

    public static void Set(double xMax, double xMin, double yMax, double yMin)
    {
        XMax = xMax;
        XMin = xMin;
        YMax = yMax;
        YMin = yMin;

        Console.WriteLine($"Clippingfenster: x: {xMin}..{xMax}; y: {yMin}..{yMax}");
    }
}

// ReSharper disable CommentTypo
public static class LineClipping
{
    private const byte Left = 1;
    private const byte Right = 2;
    private const byte Bottom = 4;
    private const byte Top = 8;

    public static byte RegionCode(Point2d p)
    {
        byte rc = 0;

        if (p.X < ClippingWindow.XMin)
            rc = Left;
        else if (p.X > ClippingWindow.XMax)
            rc = Right;

        if (p.Y < ClippingWindow.YMin)
            rc += Top;
        else if (p.Y > ClippingWindow.YMax)
            rc += Bottom;

        return rc;
    }

    /// <summary>
    /// Der Algorithmus Berechnet die Schnittpunkte zwischen einer Linie und einem ClippingFenster
    ///
    /// - Liegen beide Punkte p0 und p1 innerhalb des Fensters, bleiben die Punkte unverändert und der Algorithmus gibt true zurück
    /// - Liegen beide Punkte p0 und p1 außerhalb des Fensters, gibt der Rückgabewert an, ob die Linie durch das Fenster läuft (return true) oder nicht (return false)
    /// - Liegen die Punkte so, dass die Linie durch das Fenster läuft, berechnet der Algorithmus neue Punkte (bzw. p0 und p1 werden aktualisiert), die auf den Kanten
    ///   des Clippingfensters liegen. Mit den neu berechneten Punkten p0 und p1 lässt sich dann die Linie effizient innerhalb vom Clippingfenster zeichnen.
    /// 
    /// </summary>
    /// <param name="p0">Der Startpunkt der Linie</param>
    /// <param name="p1">Der Endpunkt der Linie</param>
    /// <returns>True, wenn die Linie durch das Clipping geht, ansonsten false</returns>
    public static bool CohenSutherland(Point2d p0, Point2d p1)
    {
        // Der region code bestimmt, wo die Punkte relativ zum ClippingFenster liegen
        var rc0 = RegionCode(p0);
        var rc1 = RegionCode(p1);

        // beinhaltet die Abbruchbedingung für die Schleife
        var done = false;

        // Rückgabewert, ob linie durchs Clippingfenster läuft
        var accept = false;

        do
        {
            if (rc0 == 0 && rc1 == 0)
            {
                // Wenn der regionCode von beiden punkten 0 ist,
                // dann sind beide Punkte im Clippingfenster und man muss nichts mehr tun
                done = true;                                            
                accept = true;                                      
            }                                                       
            else if ((rc0 & rc1) != 0)
            {
                // Wenn die bitweise Konjunktion nicht 0 ergibt, führt die Linie nicht durchs Clippingfenster
                done = true;
                accept = false;
            }
            else // rc0 oder rc1 sind != 0 UND die Bitweise verundung von rc0 und rc1 ergeben 0
            {
                byte outCode;

                // schaue in der schleife entweder p0 oder p1 an
                if (rc0 != 0)
                    outCode = rc0;
                else
                    outCode = rc1;

                double x;
                double y;

                if ((Left & outCode) == Left)
                {
                    // Wenn mein punkt links vom Clippingfenster liegt,
                    // dann ist die X-Koordinate von dem neuen Punkt auf jeden fall die linke Position vom Fenster
                    x = ClippingWindow.XMin;

                    // Aus ZeichneLinie1:
                    // double m = (v1 − v0)/(u1 − u0)
                    //
                    // mit u = x und v = y:
                    // double m = (y1 − y0)/(x1 − x0)
                    //
                    // und mit Punkten p0 und p1:
                    // double m = (p1.Y − p0.Y)/(p1.X − p0.X)
                    //
                    // auf p0.Y muss man jetzt noch was drauf addieren, weil der Punkt auf der Line weiter nach rechts gewandert ist.
                    // 
                    // Das deltaY berechnet sich aus deltaX * steigung, also (p0.xNeu - p0.xAlt)*m
                    // mit p0.xNeu = xmin und m = (p1.Y − p0.Y)/(p1.X − p0.X) ergibt deltaY dann:
                    //            (xmin - p0.X) *        (p1.Y − p0.Y)         / (p1.X − p0.X)
                    
                    y = p0.Y + (ClippingWindow.XMin - p0.X) * (p1.Y - p0.Y) / (p1.X - p0.X);
                    // und aus irgendeinem Grund hat der Fotzkopf (xmin - p0.X) und (p1.Y − p0.Y) vertauscht -.-
                }
                else if ((Right & outCode) == Right)
                {
                    // Punkt rechts vom Fenster, also xNeu = rechter Fensterrand
                    x = ClippingWindow.XMax;

                    // berechne neues y, indem auf p0.Y wieder (wie bei Links) ein deltaY draufgerechnet wird
                    // !!!Obacht!!! xNeu ist xMax, weil rechte Fensterkante!!!
                    y = p0.Y + (p1.Y - p0.Y) * (ClippingWindow.XMax - p0.X) / (p1.X - p0.X);
                }
                else if ((Bottom & outCode) == Bottom)
                {
                    // Wenn wir unterhalb vom Fenster sind, ist die Y-Koordinate bekannt (= untere Fensterkante)
                    y = ClippingWindow.YMin;

                    // x muss jetzt halt analog ausgerechnet werden...
                    // glaube, er macht hier einfach n achsentausch x <-> y und rechnet dann wie immer...
                    x = p0.X + (p1.X - p0.X) * (ClippingWindow.YMin - p0.Y) / (p1.Y - p0.Y);
                }
                else //if ((Top & outCode) == Top)
                {
                    // oberhalb vom Fenster, also ez pz obere Fensterkante
                    y = ClippingWindow.YMax;

                    // und dann lustig p0.X + deltaX
                    x = p0.X + (p1.X - p0.X) * (ClippingWindow.YMax - p0.Y) / (p1.Y - p0.Y);
                }

                if (outCode == rc0)
                {
                    // schau, ob wir p0 oder p1 neu berechnet haben, dann punkt aktualisieren
                    p0.X = x;
                    p0.Y = y;
                    
                    // neuer regioncode, weil punkt könnte ja links und unten gewesen sein!
                    rc0 = RegionCode(p0);
                }
                else
                {
                    // wenns nicht p0 war, muss es p1 gewesen sein
                    p1.X = x;
                    p1.Y = y;

                    // neuer regioncode, weil punkt könnte ja rechts und oben gewesen sein!
                    rc1 = RegionCode(p1);
                }
            }

        } while (!done);

        return accept;
    }
}
