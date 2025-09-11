//
//  SAAgeGate.h
//  Pods
//
//  Created by Gabriel Coman on 10/08/2017.
//
//

#import <UIKit/UIKit.h>

//
// define bumper callback
typedef void (^saagegatecallback)(NSInteger selectedAge);

@interface SAAgeGate : UIViewController

+ (void) play;
+ (void) overrideLogo:(UIImage*) image;
+ (void) overrideName:(NSString*) name;

+ (void) setCallback:(saagegatecallback) callback;

+ (NSInteger) getCurrentAge;

@end
