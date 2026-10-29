#import <UIKit/UIKit.h>
#import <CoreHaptics/CoreHaptics.h>
#import <math.h>

static UIImpactFeedbackGenerator *sLight;
static UIImpactFeedbackGenerator *sMedium;
static UIImpactFeedbackGenerator *sHeavy;
static UINotificationFeedbackGenerator *sNotify;
static UISelectionFeedbackGenerator *sSelect;
static CHHapticEngine *sEngine;
static id<CHHapticAdvancedPatternPlayer> sLoopPlayer;

static void GridAuraPrepareKit(void)
{
    if (@available(iOS 10.0, *))
    {
        if (sLight == nil)
            sLight = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleLight];
        if (sMedium == nil)
            sMedium = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleMedium];
        if (sHeavy == nil)
            sHeavy = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleHeavy];
        if (sNotify == nil)
            sNotify = [[UINotificationFeedbackGenerator alloc] init];
        if (sSelect == nil)
            sSelect = [[UISelectionFeedbackGenerator alloc] init];
        [sLight prepare];
        [sMedium prepare];
        [sHeavy prepare];
        [sNotify prepare];
        [sSelect prepare];
    }
}

static void GridAuraEnsureEngine(void)
{
    if (@available(iOS 13.0, *))
    {
        if (sEngine != nil)
            return;
        NSError *err = nil;
        sEngine = [[CHHapticEngine alloc] initAndReturnError:&err];
        if (sEngine == nil)
            return;
        sEngine.playsHapticsOnly = YES;
        [sEngine startAndReturnError:&err];
        sEngine.stoppedHandler = ^(CHHapticEngineStoppedReason reason) {
            sEngine = nil;
            sLoopPlayer = nil;
        };
        sEngine.resetHandler = ^{
            [sEngine startAndReturnError:nil];
        };
    }
}

static void GridAuraPlayEvents(NSArray<CHHapticEvent *> *events)
{
    if (@available(iOS 13.0, *))
    {
        GridAuraEnsureEngine();
        if (sEngine == nil)
            return;
        NSError *err = nil;
        CHHapticPattern *pattern = [[CHHapticPattern alloc] initWithEvents:events parameterCurves:@[] error:&err];
        if (pattern == nil)
            return;
        id<CHHapticPatternPlayer> player = [sEngine createPlayerWithPattern:pattern error:&err];
        [player startAtTime:0 error:&err];
    }
}

extern "C" void GridAuraHapticLight()
{
    GridAuraPrepareKit();
    if (@available(iOS 10.0, *))
        [sLight impactOccurred];
}

extern "C" void GridAuraHapticMedium()
{
    GridAuraPrepareKit();
    if (@available(iOS 10.0, *))
        [sMedium impactOccurred];
}

extern "C" void GridAuraHapticHeavy()
{
    GridAuraPrepareKit();
    if (@available(iOS 10.0, *))
        [sHeavy impactOccurred];
}

extern "C" void GridAuraHapticTick()
{
    GridAuraPrepareKit();
    if (@available(iOS 10.0, *))
        [sSelect selectionChanged];
}

extern "C" void GridAuraHapticMiss()
{
    GridAuraPrepareKit();
    if (@available(iOS 10.0, *))
        [sNotify notificationOccurred:UINotificationFeedbackTypeError];
}

extern "C" void GridAuraHapticHint()
{
    if (@available(iOS 13.0, *))
    {
        CHHapticEventParameter *intense = [[CHHapticEventParameter alloc] initWithParameterID:CHHapticEventParameterIDHapticIntensity value:0.9f];
        CHHapticEventParameter *sharp = [[CHHapticEventParameter alloc] initWithParameterID:CHHapticEventParameterIDHapticSharpness value:0.7f];
        NSArray *params = @[intense, sharp];
        CHHapticEvent *a = [[CHHapticEvent alloc] initWithEventType:CHHapticEventTypeHapticTransient parameters:params relativeTime:0];
        CHHapticEvent *b = [[CHHapticEvent alloc] initWithEventType:CHHapticEventTypeHapticTransient parameters:params relativeTime:0.09];
        CHHapticEventParameter *soft = [[CHHapticEventParameter alloc] initWithParameterID:CHHapticEventParameterIDHapticIntensity value:0.45f];
        CHHapticEvent *c = [[CHHapticEvent alloc] initWithEventType:CHHapticEventTypeHapticContinuous parameters:@[soft, sharp] relativeTime:0.16 duration:0.12];
        GridAuraPlayEvents(@[a, b, c]);
        return;
    }
    GridAuraPrepareKit();
    if (@available(iOS 10.0, *))
    {
        [sMedium impactOccurred];
        dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(0.09 * NSEC_PER_SEC)), dispatch_get_main_queue(), ^{
            [sHeavy impactOccurred];
        });
    }
}

extern "C" void GridAuraHapticConfettiStart()
{
    GridAuraPrepareKit();
    if (@available(iOS 10.0, *))
        [sHeavy impactOccurred];
}

extern "C" void GridAuraHapticStop()
{
    [sLoopPlayer stopAtTime:0 error:nil];
    sLoopPlayer = nil;
}
