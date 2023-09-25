

$(window).on("load", function () {
    if (location.href.indexOf('view=dev') > 0) {
        if ($('.data').data('islogin') == 'False') {
            setTimeout(function () {
                firebase.init()
            }, 500);
        } else {
            setTimeout(() => {
                firebase.initLogin();
            }, 500)
        }
    }
})

$(document).on('click', '.logout', function () {
    $.cookie("userToken", null);
    $.removeCookie("userToken", { path: '/' });

    location.href = location.href;
});

var firebase = {
    init: async function () {
        await $('<link>')
            .appendTo('head')
            .attr({
                type: 'text/css',
                rel: 'stylesheet',
                href: '../lib/firebaseui/dist/firebaseui.css'
            });
        await $.getScript("../lib/firebase/firebase-app-compat.js");
        await $.getScript("../lib/firebaseui/dist/firebaseui.js");
        await $.getScript("../lib/firebase/firebase-auth-compat.js");
        firebase.initializeApp({
            apiKey: 'AIzaSyAC14T-jU96RqlVo5nlkL92W9n9y3akniM',
            authDomain: 'jin-nie.firebaseapp.com',
            projectId: 'jin-nie',
            storageBucket: 'jin-nie.appspot.com',
            messagingSenderId: '543334601384',
            appId: '1:543334601384:web:271e306a00d4113c5ce89d',
            measurementId: 'G-414XY990V0',
        });
        var ui = new firebaseui.auth.AuthUI(firebase.auth());
        var uiConfig = {
            callbacks: {
                signInSuccessWithAuthResult: function (authResult, redirectUrl) {
                    firebase.auth().currentUser.getIdToken(/* forceRefresh */ true).then(function (idToken) {
                        $.ajax({
                            url: "/Auth",
                            data: { token: idToken }
                            , success: function (result) {                              
                                $('#firebaseui-auth-container').remove();
                                $('#user-login-nav').append(result);
                                console.log(authResult);
                                setTimeout(() => {
                                    const toastcon = ` <div class="position-fixed top-0 end-0 p-3" style="z-index: 11">
            <div id="liveToast" class="toast hide" role="alert" aria-live="assertive" aria-atomic="true" data-bs-autohide="true">
                <div class="toast-header">
                   
                    
                    <small>just now</small>
                    <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
                <div class="toast-body">
                    Wellcome ${authResult.user.phoneNumber}
                </div>
            </div>
        </div>`
                                    $('.bootstrapinit').append(toastcon);
                                    test = $('#liveToast');
                                    const toast = bootstrap.Toast.getOrCreateInstance(test);
                                    toast.show();
                                }, 50)


                            }

                        });
                    }).catch(function (error) {
                        // Handle error
                    });




                    // User successfully signed in.
                    // Return type determines whether we continue the redirect automatically
                    // or whether we leave that to developer to handle.
                    return false;
                },
                uiShown: function () {
                    // The widget is rendered.
                    // Hide the loader.
                },
            },
            // Will use popup for IDP Providers sign-in flow instead of the default, redirect.
            signInFlow: 'popup',

            signInOptions: [
                {
                    provider: firebase.auth.PhoneAuthProvider.PROVIDER_ID,
                    recaptchaParameters: {
                        type: 'image', // 'audio'
                        size: 'normal', // 'invisible' or 'compact'
                        badge: 'bottomleft' //' bottomright' or 'inline' applies to invisible.
                    },
                    defaultCountry: 'VN', // Set default country to the United Kingdom (+44).
                    // For prefilling the national number, set defaultNationNumber.
                    // This will only be observed if only phone Auth provider is used since
                    // for multiple providers, the NASCAR screen will always render first
                    // with a 'sign in with phone number' button.
                }
            ],
        };
        ui.start('#firebaseui-auth-container', uiConfig);
    },
    initLogin: function () {
        
        
        
        setTimeout(() => {
            $.ajax({
                url: "/Auth/InitLogin",
                success: function (result) {
                    if (result.code != 200) {
                        
                    } else {
                        var data = JSON.parse(result.data);
                        const toastcon = ` <div class="position-fixed top-0 end-0 p-3" style="z-index: 11">
            <div id="liveToast" class="toast hide" role="alert" aria-live="assertive" aria-atomic="true" data-bs-autohide="true">
                <div class="toast-header">
                    
                    <strong class="me-auto">${data.phoneNumber}</strong>
                    <small>just now</small>
                    <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
                <div class="toast-body">
                    Wellcome ${data.userName}
                </div>
            </div>
        </div>`
                        $('.bootstrapinit').append(toastcon);
                        test = $('#liveToast');
                        const toast = bootstrap.Toast.getOrCreateInstance(test);
                        toast.show();
                    }

                }

            });
        }, 50)
    }
}
