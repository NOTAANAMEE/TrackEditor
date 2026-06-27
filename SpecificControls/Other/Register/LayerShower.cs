using SpecificControls.Canvases;
using SpecificControls.Graph;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace SpecificControls.Other;

public partial class LayerShower
{
    public static readonly DependencyProperty CellInfoCollectionProperty
        = DependencyProperty.Register(
            nameof(CellInfoCollection), typeof(ObservableCollection<IInfo>),
            typeof(LayerShower), new PropertyMetadata(null, CellInfoCollectionChange));

    private static void CellInfoCollectionChange(DependencyObject d, 
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not LayerShower shower) return;
        shower.HandleReset();
        var collection = e.OldValue as ObservableCollection<IInfo>;
        collection?.CollectionChanged -= shower.CellInfoCollection_CollectionChanged;
        collection = e.NewValue as ObservableCollection<IInfo>;
        collection?.CollectionChanged += shower.CellInfoCollection_CollectionChanged;
    }

    public static readonly DependencyProperty SelectedCellProperty
        = DependencyProperty.Register(
            nameof(SelectedCell), typeof(CellInfo), typeof(LayerShower),
            new PropertyMetadata(null, SelectedCellChanged));

    private static void SelectedCellChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not LayerShower shower) return;
        shower.UpdateLayerVisuals();
    }

    public static readonly DependencyProperty ScaleXProperty
        = DependencyProperty.Register(
            nameof(ScaleX), typeof(double), typeof(LayerShower),
            new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty ScaleYProperty
        = DependencyProperty.Register(
            nameof(ScaleY), typeof(double), typeof(LayerShower),
            new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty CenterProperty
        = DependencyProperty.Register(
            nameof(Center), typeof(Point), typeof(LayerShower),
            new FrameworkPropertyMetadata(new Point(0, 0),
                FrameworkPropertyMetadataOptions.AffectsRender));
}
