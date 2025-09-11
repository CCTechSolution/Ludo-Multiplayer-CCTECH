#import <UIKit/UIKit.h>
#import "SAgUnityCallback.h"

#if defined(__has_include)
#if __has_include(<SAAgeGateSDK/SAAgeGate.h>)
#import <SAAgeGateSDK/SAAgeGate.h>
#else
#import "SAAgeGate.h"
#endif
#endif

extern "C" {
    
    /**
     * Unity to native iOS method that sends a CPI event.
     */
    void SuperAwesomeAgeGateUnitySAOverrideName (const char *cName) {
        
        NSString *name = [NSString stringWithUTF8String:cName];
        [SAAgeGate overrideName:name];
    }
    
    void SuperAwesomeAgeGateUnitySAPlay () {
        
        [SAAgeGate setCallback:^(NSInteger selectedAge) {
            sendAgeGateCallback(@"SAAgeGate", (int)(selectedAge), @"HandleAge");
        }];
        
        [SAAgeGate play];
    }
    
    int SuperAwesomeAgeGateUnitySAGetCurrentAge () {
        return (int)[SAAgeGate getCurrentAge];
    }
}
