#import <UIKit/UIKit.h>

static void GridAuraFireImpact(UIImpactFeedbackStyle style)
{
    if (@available(iOS 10.0, *))
    {
        UIImpactFeedbackGenerator *generator = [[UIImpactFeedbackGenerator alloc] initWithStyle:style];
        [generator prepare];
        [generator impactOccurred];
    }
}

extern "C" void GridAuraHapticLight()
{
    GridAuraFireImpact(UIImpactFeedbackStyleLight);
}

extern "C" void GridAuraHapticMedium()
{
    GridAuraFireImpact(UIImpactFeedbackStyleMedium);
}

extern "C" void GridAuraHapticHeavy()
{
    GridAuraFireImpact(UIImpactFeedbackStyleHeavy);
}
