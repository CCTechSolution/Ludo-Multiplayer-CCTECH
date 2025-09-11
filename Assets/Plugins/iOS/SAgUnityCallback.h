#import <UIKit/UIKit.h>

// forward declaration of this method - which is part of the Unity C
// libray, so it would be available there
void UnitySendMessage(const char *identifier, const char *function, const char *payload);

/**
 * Generic method used to send messages back to unity
 *
 * @param unityName the name of the unity ad to send the message back to
 * @param payload   the data to send back
 */
static inline void sendToUnity2 (NSString *unityName, NSString *payload) {
    
    const char *name = [unityName UTF8String];
    const char *payloadUTF8 = [payload UTF8String];
    UnitySendMessage (name, "nativeCallback", payloadUTF8);
    
}

/**
 * Method that sends back CPU data to Unity
 *
 * @param unityName     the name of the unity ad to send the message back to
 * @param age           user's age
 * @param callback      callback method
 */
static inline void sendAgeGateCallback (NSString *unityName, int age, NSString *callback) {
    
    NSString *payload = [NSString stringWithFormat:@"{\"age\":\"%d\", \"type\":\"sacallback_%@\"}", age, callback];
    sendToUnity2(unityName, payload);
}
