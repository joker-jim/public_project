#include <opencv2/opencv.hpp>
#include <opencv2/ml.hpp>
#include <iostream>
#include <filesystem>
#include <vector>
#include <random>
#include <chrono>

using namespace cv;
using namespace cv::ml;
using namespace std;
namespace fs = std::filesystem;

Mat extractFeatures(const Mat& image) {
    Mat resizedImage;
    resize(image, resizedImage, Size(700, 700)); 
    Mat featureVector;
    resizedImage.reshape(1, 1).convertTo(featureVector, CV_32F);
    return featureVector;
}


Mat skinDetection(const Mat& image) {
    Mat ycrcb;
    cvtColor(image, ycrcb, COLOR_BGR2YCrCb); 
    vector<Mat> channels;
    split(ycrcb, channels);

    Mat cr = channels[1];
    Mat cb = channels[2];

    Mat skin;
    skin = Mat::zeros(cr.size(), CV_8UC1);

    for (int i = 0; i < skin.rows; i++) {
        for (int j = 0; j < skin.cols; j++) {
            /*light*///if ((cr.at<uchar>(i, j) >= 145 && cr.at<uchar>(i, j) <= 165) && (cb.at<uchar>(i, j) >= 112 && cb.at<uchar>(i, j) <= 148)) {
            /*dark*/if ((cr.at<uchar>(i, j) >= 135 && cr.at<uchar>(i, j) <= 200) && (cb.at<uchar>(i, j) >= 80 && cb.at<uchar>(i, j) <= 126)) {
                skin.at<uchar>(i, j) = 255;
            }
        }
    }

    Mat denoisedSkin;
    morphologyEx(skin, denoisedSkin, MORPH_OPEN, getStructuringElement(MORPH_ELLIPSE, Size(5, 5)));

    return denoisedSkin;
}

int main() {
    Ptr<RTrees> model = Algorithm::load<RTrees>("random_forest_model.xml");

    string testFolderPath = "/home/parallels/Desktop/opencv_project/3/Test set for gestures 0 to 9-20230531";
    bool useCamera = false;
    string imagePath;

    cout << "Enter the path of the image (leave blank to use camera): ";
    getline(cin, imagePath);

    if (imagePath.empty()) {
        useCamera = true;
    }

    if (useCamera) {
        VideoCapture cap(0);
        if (!cap.isOpened()) {
            cerr << "ERROR: Unable to open the camera" << endl;
            return 0;
        }

        cap.set(CAP_PROP_BRIGHTNESS, 0.5);
        cap.set(CAP_PROP_CONTRAST, 0.5);
        cap.set(CAP_PROP_SATURATION, 0.5);

        const int frameDelay = 1000 / 60;

        while (true) {
            auto start = chrono::high_resolution_clock::now();

            Mat frame;
            cap >> frame;

            GaussianBlur(frame, frame, Size(3, 3), 0);

            Mat skinMask = skinDetection(frame);

            vector<vector<Point>> contours;
            findContours(skinMask, contours, RETR_EXTERNAL, CHAIN_APPROX_SIMPLE);

            int maxContourIndex = 0;
            double maxContourArea = 0.0;
            for (int i = 0; i < contours.size(); i++) {
                double currentContourArea = contourArea(contours[i]);
                if (currentContourArea > maxContourArea) {
                    maxContourArea = currentContourArea;
                    maxContourIndex = i;
                }
            }

            Mat result = Mat::zeros(frame.size(), CV_8UC3);
            drawContours(result, contours, maxContourIndex, Scalar(255, 255, 255), FILLED);

            Mat binaryImage;
            cvtColor(result, binaryImage, COLOR_BGR2GRAY);
            threshold(binaryImage, binaryImage, 1, 255, THRESH_BINARY);

            vector<vector<Point>> binaryContours;
            findContours(binaryImage, binaryContours, RETR_EXTERNAL, CHAIN_APPROX_SIMPLE);

            int maxBinaryContourIndex = 0;
            double maxBinaryContourArea = 0.0;
            for (int i = 0; i < binaryContours.size(); i++) {
                double currentBinaryContourArea = contourArea(binaryContours[i]);
                if (currentBinaryContourArea > maxBinaryContourArea) {
                    maxBinaryContourArea = currentBinaryContourArea;
                    maxBinaryContourIndex = i;
                }
            }

            if (!binaryContours.empty()) {
                Rect boundingRectROI = boundingRect(binaryContours[maxBinaryContourIndex]);
                Mat resizedImage;
                resize(binaryImage(boundingRectROI), resizedImage, Size(400, 500));
                Mat finalResult = Mat::zeros(Size(400, 500), CV_8UC1);
                int startX = (finalResult.cols - resizedImage.cols) / 2;
                int startY = (finalResult.rows - resizedImage.rows) / 2;
                resizedImage.copyTo(finalResult(Rect(startX, startY, resizedImage.cols, resizedImage.rows)));

                Mat testFeatureVector = extractFeatures(finalResult);
                float predictedLabel = model->predict(testFeatureVector);
                auto finish = chrono::high_resolution_clock::now();

                chrono::duration<double> elapsed = finish - start;
                double fps = 1.0 / elapsed.count();

                stringstream ss;
                ss << "FPS: " << fps << ", number: " << predictedLabel;
                putText(frame, ss.str(), Point(10, 30), FONT_HERSHEY_SIMPLEX, 1, Scalar(0, 0, 255), 2);

                imshow("Original Image", frame);
                imshow("Skin Mask", skinMask);
                imshow("Binary Image", binaryImage);
                imshow("Final Result", finalResult);
            } else {
                stringstream ss;
                ss << "No contours found";
                putText(frame, ss.str(), Point(10, 30), FONT_HERSHEY_SIMPLEX, 1, Scalar(0, 0, 255), 2);
                imshow("Original Image", frame);
            }

            auto finish = chrono::high_resolution_clock::now();
            chrono::duration<double> elapsed = finish - start;
            double frameTime = elapsed.count() * 1000;

            if (frameTime < frameDelay) {
                int delay = static_cast<int>(frameDelay - frameTime);
                if (waitKey(delay) == 27) {
                    break;
                }
            } else {
                if (waitKey(1) == 27) {
                    break;
                }
            }
        }
    } else {
        Mat testImage = imread(imagePath);
        Mat skinMask = skinDetection(testImage);

        vector<vector<Point>> contours;
        findContours(skinMask, contours, RETR_EXTERNAL, CHAIN_APPROX_SIMPLE);

        int maxContourIndex = 0;
        double maxContourArea = 0.0;
        for (int i = 0; i < contours.size(); i++) {
            double currentContourArea = contourArea(contours[i]);
            if (currentContourArea > maxContourArea) {
                maxContourArea = currentContourArea;
                maxContourIndex = i;
            }
        }

        Mat result = Mat::zeros(testImage.size(), CV_8UC3);
        drawContours(result, contours, maxContourIndex, Scalar(255, 255, 255), FILLED);

        Mat binaryImage;
        cvtColor(result, binaryImage, COLOR_BGR2GRAY);
        threshold(binaryImage, binaryImage, 1, 255, THRESH_BINARY);

        vector<vector<Point>> binaryContours;
        findContours(binaryImage, binaryContours, RETR_EXTERNAL, CHAIN_APPROX_SIMPLE);

        int maxBinaryContourIndex = 0;
        double maxBinaryContourArea = 0.0;
        for (int i = 0; i < binaryContours.size(); i++) {
            double currentBinaryContourArea = contourArea(binaryContours[i]);
            if (currentBinaryContourArea > maxBinaryContourArea) {
                maxBinaryContourArea = currentBinaryContourArea;
                maxBinaryContourIndex = i;
            }
        }

        Rect boundingRectROI = boundingRect(binaryContours[maxBinaryContourIndex]);
        Mat resizedImage;
        resize(binaryImage(boundingRectROI), resizedImage, Size(400, 500));
        Mat finalResult = Mat::zeros(Size(400, 500), CV_8UC1);
        int startX = (finalResult.cols - resizedImage.cols) / 2;
        int startY = (finalResult.rows - resizedImage.rows) / 2;
        resizedImage.copyTo(finalResult(Rect(startX, startY, resizedImage.cols, resizedImage.rows)));

        Mat testFeatureVector = extractFeatures(finalResult);
        float predictedLabel = model->predict(testFeatureVector);
        cout << "Image: " << imagePath << ", Number: " << predictedLabel << endl;

        imshow("Original Image", testImage);
        imshow("Hand Contour", finalResult);
        waitKey(0);
    }

    return 0;
}
