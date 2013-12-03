namespace Digits

open System.Windows.Forms
open System.Drawing

module Visualizer =

    let tile = 20

    let draw (digit:int*float[]) (text:string) =

        let form = new Form(TopMost = true, Visible = true, Width = 29 * tile, Height = 29 * tile)
                   
        let panel = new Panel(Dock = DockStyle.Fill)
        panel.BackColor <- Color.Black
        form.Controls.Add(panel)

        let graphics = panel.CreateGraphics()
        
        let n, pixels = digit
        
        pixels 
        |> Array.iteri (fun i p ->
            let col = i % 28
            let row = i / 28
            let color = Color.FromArgb(int p, int p, int p)
            let brush = new SolidBrush(color)
            graphics.FillRectangle(brush,col*tile,row*tile,tile,tile))

        let point = new PointF((float32)5, (float32)5)
        let font = new Font(family = FontFamily.GenericSansSerif, emSize = (float32)30)        
        graphics.DrawString(text, font, new SolidBrush(Color.Red), point)
        
        form.Show()