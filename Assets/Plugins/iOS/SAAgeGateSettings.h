//
//  SATriggerOnce.h
//  Pods
//
//  Created by Gabriel Coman on 11/08/2017.
//
//

#import <UIKit/UIKit.h>

@interface SAAgeGateSettings : NSObject

//
// trigger once
+ (BOOL) isTriggered;
+ (void) reset;

//
// age static methods
+ (void) setAge:(NSInteger) age;
+ (NSInteger) getAge;

@end
